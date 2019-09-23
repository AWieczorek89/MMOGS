using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using System;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector;
using UnityEngine;

namespace BackgroundManagement.DataHandlers
{
    public class ConnectionChecker
    {
        public enum PingRequestState
        {
            Out,
            In
        }

        public enum LoginState
        {
            NotLoggedInOrLoginFailed,
            Logged,
            WaitingForResponse
        }

        private static readonly object _dataLock = new object();
        private static readonly int _connCheckIntervalMs = 3000;
        private static readonly int _pingCheckIntervalMs = 1000;
        private static readonly int _pingMaxLimit = 9999;
        private static ConnectionChecker _lastInstance = null;
        
        private PingRequestState _pingState = PingRequestState.Out;
        private LoginState _clientLoginState = LoginState.NotLoggedInOrLoginFailed;
        public LoginState ClientLoginState
        {
            get { lock (_dataLock) { return _clientLoginState; } }
            private set { lock (_dataLock) { _clientLoginState = value; } }
        }

        private int _loginAttemptCount = 0;
        private bool _disconnectionFlag = false;

        private int _pingId = 0;
        private DateTime _sentPingTime = DateTime.Now;
        
        private int _pingMs = 0;
        public int PingMs
        {
            get { lock (_dataLock) { return _pingMs; } }
            private set { lock (_dataLock) { _pingMs = value; } }
        }

        private bool _isUnlocked = false;
        public bool IsUnlocked
        {
            get { lock (_dataLock) { return _isUnlocked; } }
            private set { lock (_dataLock) { _isUnlocked = value; } }
        }

        private DateTime _serverTime = DateTime.Now;
        public DateTime ServerTime
        {
            get { lock (_dataLock) { return _serverTime; } }
            set { lock (_dataLock) { _serverTime = value; } }
        }

        public ConnectionChecker()
        {
            _lastInstance = this;
            CheckConnectionAsync();
            CheckPingAsync();
        }

        public static void Unlock(bool unlock)
        {
            if (_lastInstance == null)
            {
                MainGameHandler.GetChatHandler().UpdateLog("Connection checker - Unlock() - checker instance is NULL!");
                return;
            }

            _lastInstance.IsUnlocked = unlock;
        }

        public void SetLoginState(LoginState state)
        {
            this.ClientLoginState = state;

            switch (state)
            {
                case LoginState.NotLoggedInOrLoginFailed:
                    {
                        _loginAttemptCount++;

                        Action closeAppAction = null;
                        if (_loginAttemptCount >= 3)
                            closeAppAction = () => { MainGameHandler.CloseApplication(); };

                        MainGameHandler.ShowMessageBox($"Failed to log in ({_loginAttemptCount}/3)", "Login failed", closeAppAction);
                    }
                    break;
                case LoginState.Logged:
                    {
                        _loginAttemptCount = 0;
                        Action changeSceneAction = () => { MainGameHandler.ChangeScene(MainGameHandler.SceneType.CharLobby); };
                        MainGameHandler.ShowMessageBox($"You have successfully logged in!", "Login success", changeSceneAction);
                    }
                    break;
                case LoginState.WaitingForResponse:
                    //Debug.Log("Login - waiting for response");
                    break;
            }
        }

        private void ShowDisconnectionMessage(string reason = "")
        {
            if (_disconnectionFlag)
                return;

            _disconnectionFlag = true;
            Action closingAction = () => { MainGameHandler.CloseApplication(); };
            MainGameHandler.ShowMessageBox
            (
                $"Cannot connect to the server!{(!String.IsNullOrWhiteSpace(reason) ? $" Reason: {reason}" : "")}", 
                "Connection problem", 
                closingAction
            );
        }

        private async void CheckConnectionAsync()
        {
            do
            {
                if (MainGameHandler.ApplicationCloseCalled)
                    break;

                if (this.IsUnlocked && !TcpServerClient.CheckIfConnected())
                {
                    ShowDisconnectionMessage();
                    break;
                }
                
                await Task.Factory.StartNew(() => Thread.Sleep(_connCheckIntervalMs));
            }
            while (!MainGameHandler.ApplicationCloseCalled);
        }

        public static void SetPingResponse(int pingId, DateTime timeArrival)
        {
            ConnectionChecker instance = _lastInstance;
            if (instance == null)
                return;

            if (instance._pingState == PingRequestState.Out || instance._pingId != pingId)
                return;

            instance.PingMs = Convert.ToInt32(((TimeSpan)(timeArrival - instance._sentPingTime)).TotalMilliseconds);
            instance._pingState = PingRequestState.Out;
        }

        private async void CheckPingAsync()
        {
            TimeSpan tSpan;

            do
            {
                if (MainGameHandler.ApplicationCloseCalled)
                    break;

                if (_pingState == PingRequestState.Out && this.IsUnlocked)
                {
                    _pingId++;
                    _sentPingTime = DateTime.Now;
                    CommandHandler.Send(new PingRequestCmdBuilder(_pingId));
                    _pingState = PingRequestState.In;
                }
                else
                if (_pingState == PingRequestState.In && this.IsUnlocked)
                {
                    tSpan = DateTime.Now - _sentPingTime;
                    if (Math.Abs(tSpan.TotalMilliseconds) > _pingMaxLimit)
                    {
                        this.PingMs = _pingMaxLimit;
                        _pingState = PingRequestState.Out;
                    }
                }

                await Task.Factory.StartNew(() => Thread.Sleep(_pingCheckIntervalMs));
            }
            while (!MainGameHandler.ApplicationCloseCalled);
        }
    }
}
