using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using TcpConnector;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CommandHandler : IDisposable
    {
        private ILogger _logger;
        private List<ICommandHandlingStrategy> _cmdHandlingStrategyList = new List<ICommandHandlingStrategy>();

        public CommandHandler(ILogger logger)
        {
            _logger = logger ?? throw new Exception("(Command handler) logger cannot be NULL!");
        }

        public void RegisterCommandHandlingStrategy(ICommandHandlingStrategy strategy)
        {
            _cmdHandlingStrategyList.Add(strategy);
        }

        public void ClearCommandHandlingStrategies()
        {
            _cmdHandlingStrategyList.Clear();
        }
        
        public static void Send(ICommandBuilder cmdBuilder, PlayerDetails playerDetails)
        {
            if (cmdBuilder == null || playerDetails == null)
                throw new Exception("command sending error - cmd builder or player details object is NULL!");

            CommandCreator.CreateCommand(cmdBuilder);
            TcpServer.SendCommandToClient(cmdBuilder.GetCommand(), playerDetails.TcpClientId);
        }

        public bool ExecuteCommand(ClientCommandInfo cmdInfo, PlayerDetails playerDetails)
        {
            bool executed = false;

            try
            {
                foreach (ICommandHandlingStrategy strategy in _cmdHandlingStrategyList)
                {
                    if (strategy.ValidateExecution(cmdInfo))
                    {
                        executed = strategy.Execute(playerDetails);
                        if (executed)
                        {
                            //_logger.UpdateLog($"CMD [{cmdInfo.CommandTxt}] executed by [{strategy.GetType().ToString()}]");
                            break;
                        }
                    }
                }

                if (!executed)
                    _logger.UpdateLog($"CMD [{cmdInfo.CommandTxt}] cannot be executed!");
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Command execution error, TCP client ID [{cmdInfo.ClientInfo.ClientId}]: {exception.Message} | {exception.StackTrace}");
                _logger.UpdateLog($"with command [{cmdInfo.CommandTxt}]");
            }
            
            return executed;
        }

        public void Dispose()
        {
            ClearCommandHandlingStrategies();
        }
    }
}
