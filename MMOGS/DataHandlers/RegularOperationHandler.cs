using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector;

namespace MMOGS.DataHandlers
{
    public class RegularOperationHandler : IDisposable
    {
        private static int _serverTimeCommandIntervalMs = 10000;

        private ILogger _logger;
        private bool _enabled = true;

        public RegularOperationHandler(ILogger logger)
        {
            _logger = logger ?? throw new Exception("Scheduled operation handler - logger cannot be NULL!");
            StartHandling();
        }

        private void StartHandling()
        {
            _logger.UpdateLog("Starting scheduled operation handling...");
            HandleServerTimeCommandAsync();
        }

        public void Dispose()
        {
            _enabled = false;
        }

        private async void HandleServerTimeCommandAsync()
        {
            ICommandBuilder cmdBuilder = null;

            while (_enabled)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(RegularOperationHandler._serverTimeCommandIntervalMs));

                cmdBuilder = new ServerTimeCmdBuilder();
                CommandCreator.CreateCommand(cmdBuilder);
                TcpServer.SendCommandToAllClients(cmdBuilder.GetCommand());

                cmdBuilder = null;
            }
        }


    }
}
