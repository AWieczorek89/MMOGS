using MMOGS.DataHandlers.CommandHandling;
using MMOGS.Models;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers
{
    public class TcpConnectionHandler
    {
        public enum WorkingState
        {
            Disabled,
            Enabled,
            Error
        }

        private static readonly object _dataLock = new object();
        private static TcpConnectionHandler _instance = null;
        
        private WorkingState _state = WorkingState.Disabled;
        public WorkingState State
        {
            get { lock (_dataLock) { return _state; } }
            private set { lock (_dataLock) { _state = value; } }
        }

        private int _commandHandlingTickRate = 50;
        private int _activityCheckTickRate = 30000;
        private TcpConnectionSettings _connectionSettings = null;
        private ITcpLogger _logger = null;
        private CommandHandler _commandHandler = null;
        private TcpServer _tcpServerInstance = null;
        private PlayerHandler _playerHandler = null;
        
        public static TcpConnectionHandler GetInstance(ITcpLogger logger, CommandHandler commandHandler, PlayerHandler playerHandler)
        {
            if (TcpConnectionHandler._instance == null)
                TcpConnectionHandler._instance = new TcpConnectionHandler();

            TcpConnectionHandler._instance._logger = logger;
            TcpConnectionHandler._instance._commandHandler = commandHandler;
            TcpConnectionHandler._instance._playerHandler = playerHandler;
            return TcpConnectionHandler._instance;
        }

        private TcpConnectionHandler()
        {
        }

        public void StopConnection()
        {
            try
            {
                this.State = WorkingState.Disabled;
                TcpServer.StopServer();
                _logger.UpdateLog("TCP connection stopped successfully!");
            }
            catch (Exception exception)
            {
                this.State = WorkingState.Error;
                _logger.UpdateLog($"TCP connection - connection stopping error: {exception.Message}");
            }
        }

        public void StartConnection(TcpConnectionSettings connectionSettings)
        {
            if (this.State == WorkingState.Enabled)
            {
                _logger.UpdateLog("TCP connection starting aborted - connection is already enabled!");
                return;
            }

            _connectionSettings = connectionSettings;

            try
            {
                ConnectionData connData = connectionSettings.ToConnectionData();
                //TcpServer.SetContainerSize(5);
                _tcpServerInstance = TcpServer.RunServer(connData, _logger);
                this.State = WorkingState.Enabled;

                HandleClientCommandsAsync();
                HandleClientActivityCheckAsync();
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"TCP connection - connection starting error: {exception.Message}");
                this.State = WorkingState.Error;
            }
        }

        private async void HandleClientActivityCheckAsync()
        {
            _logger.UpdateLog("Client activity check handling started!");
            List<PlayerDetails> playerDetailsList = null;
            ClientHandlerInfo clientHandlerInfo = null;
            int tcpClientId = -1;

            while (this.State == WorkingState.Enabled)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(_activityCheckTickRate));

                playerDetailsList = _playerHandler.GetAllPlayers();
                if (playerDetailsList == null)
                    continue;

                foreach (PlayerDetails details in playerDetailsList)
                {
                    tcpClientId = details.TcpClientId;
                    clientHandlerInfo = TcpServer.GetClientInfo(tcpClientId);
                    if (clientHandlerInfo == null)
                        continue;

                    if (!clientHandlerInfo.IsWorking)
                    {
                        _logger.UpdateLog($"Activity check - unregistering player with TCP client ID [{tcpClientId}]...");
                        _playerHandler.UnregisterPlayer(tcpClientId);
                    }
                }
                
                playerDetailsList.Clear();
                playerDetailsList = null;
                clientHandlerInfo = null;
            }
        }

        private async void HandleClientCommandsAsync()
        {
            _logger.UpdateLog("TCP client command handling started!");
            List<ClientCommandInfo> clientCmdList = new List<ClientCommandInfo>();
            PlayerDetails playerDetails = null;
            bool commandExecuted = false;

            while (this.State == WorkingState.Enabled)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(_commandHandlingTickRate));

                clientCmdList = TcpServer.GetCommandsFromAllCLients();
                foreach (ClientCommandInfo cmdInfo in clientCmdList)
                {
                    if (!cmdInfo.ClientInfo.IsWorking)
                    {
                        _playerHandler.UnregisterPlayer(cmdInfo.ClientInfo.ClientId);
                        continue;
                    }

                    //COMMAND HANDLING
                    playerDetails = _playerHandler.GetPlayer(cmdInfo.ClientInfo.ClientId);

                    if (playerDetails == null)
                        playerDetails = _playerHandler.RegisterPlayer(cmdInfo.ClientInfo.ClientId);

                    commandExecuted = _commandHandler.ExecuteCommand(cmdInfo, playerDetails);

                    if (!commandExecuted)
                    {
                        if (playerDetails != null)
                        {
                            if (String.IsNullOrWhiteSpace(playerDetails.Login))
                            {
                                playerDetails.LoginAttemptCount++;
                                _logger.UpdateLog($"Login attempt count set to [{playerDetails.LoginAttemptCount}] for TCP client ID [{cmdInfo.ClientInfo.ClientId}]");

                                if (playerDetails.LoginAttemptCount > 2)
                                {
                                    TcpServer.KickClient(cmdInfo.ClientInfo.ClientId);
                                    _playerHandler.UnregisterPlayer(cmdInfo.ClientInfo.ClientId);
                                    _logger.UpdateLog("Player kicked!");
                                }
                            }
                        }
                        else
                        {
                            _logger.UpdateLog($"Cannot execute command for TCP client ID [{cmdInfo.ClientInfo.ClientId}] - player details not found (NULL)!");
                        }
                    }
                }

                if (clientCmdList.Count > 0)
                    clientCmdList.Clear();
            }

            _logger.UpdateLog("TCP client command handling stopped!");
        }
    }
}
