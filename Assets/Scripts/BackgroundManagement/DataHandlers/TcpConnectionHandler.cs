using BackgroundManagement.DataHandlers.CommandHandling;
using BackgroundManagement.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector;
using TcpConnector.DataModels;

namespace BackgroundManagement.DataHandlers
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
        
        private TcpConnectionSettings _connectionSettings = null;
        private ITcpLogger _logger = null;
        private TcpServerClient _tcpClientInstance = null;
        private int _commandHandlingTickRate = 50;

        private CommandHandler _commandHandler;

        public static TcpConnectionHandler GetInstance(ITcpLogger logger, CommandHandler commandHandler)
        {
            if (TcpConnectionHandler._instance == null)
                TcpConnectionHandler._instance = new TcpConnectionHandler(logger, commandHandler);
            
            return TcpConnectionHandler._instance;
        }

        private TcpConnectionHandler(ITcpLogger logger, CommandHandler commandHandler)
        {
            _logger = logger;
            _commandHandler = commandHandler;
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
            _connectionSettings = connectionSettings;
            _logger.UpdateLog($"Starting connection with IP [{connectionSettings.Ip}] port [{connectionSettings.Port}]");

            try
            {
                ConnectionData connData = connectionSettings.ToConnectionData();
                //TcpServerClient.SetCommandInsertionTimeLimit(15);
                _tcpClientInstance = TcpServerClient.RunServerClient(connData, _logger);
                this.State = WorkingState.Enabled;

                HandleServerCommandsAsync();
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"TCP connection - connection starting error: {exception.Message} | {exception.StackTrace}");
                this.State = WorkingState.Error;
            }
        }

        private async void HandleServerCommandsAsync()
        {
            _logger.UpdateLog("TCP server command handling started!");
            string serverCommand = "";

            while (this.State == WorkingState.Enabled)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(_commandHandlingTickRate));
                serverCommand = TcpServerClient.GetCommandFromServer();

                if (!String.IsNullOrEmpty(serverCommand))
                    _commandHandler.ExecuteCommand(serverCommand);
            }

            _logger.UpdateLog("TCP server command handling stopped!");
        }
    }
}
