using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.Interfaces;
using System;
using System.Collections.Generic;
using TcpConnector;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CommandHandler : IDisposable
    {
        private IChat _chat;
        private List<ICommandHandlingStrategy> _cmdHandlingStrategyList = new List<ICommandHandlingStrategy>();

        public CommandHandler(IChat chat)
        {
            _chat = chat;
        }

        public void RegisterCommandHandlingStrategy(ICommandHandlingStrategy strategy)
        {
            _cmdHandlingStrategyList.Add(strategy);
        }

        public void ClearCommandHandlingStrategies()
        {
            _cmdHandlingStrategyList.Clear();
        }

        public static void Send(ICommandBuilder cmdBuilder)
        {
            if (cmdBuilder == null)
                throw new Exception("Command sending error - cmd builder is NULL!");

            CommandCreator.CreateCommand(cmdBuilder);
            TcpServerClient.SendCommandToServer(cmdBuilder.GetCommand());
        }

        public bool ExecuteCommand(string command)
        {
            bool executed = false;

            try
            {
                foreach (ICommandHandlingStrategy strategy in _cmdHandlingStrategyList)
                {
                    if (strategy.ValidateExecution(command))
                    {
                        executed = strategy.Execute();
                        if (executed)
                            break;
                    }
                }

                if (!executed)
                    _chat.UpdateLog($"Server command [{command}] cannot be executed!");
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Command execution error: {exception.Message}");
                _chat.UpdateLog($"with command [{command}]");
            }

            return executed;
        }

        public void Dispose()
        {
            ClearCommandHandlingStrategies();
        }
    }
}
