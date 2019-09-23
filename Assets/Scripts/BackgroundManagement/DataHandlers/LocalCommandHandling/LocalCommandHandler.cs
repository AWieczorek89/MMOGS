using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using BackgroundManagement.Interfaces;
using System;
using System.Collections.Generic;

namespace BackgroundManagement.DataHandlers.LocalCommandHandling
{
    public class LocalCommandHandler : IDisposable
    {
        private IChat _chat;
        private List<ILocalCommandHandlingStrategy> _cmdHandlingStrategyList = new List<ILocalCommandHandlingStrategy>();

        public LocalCommandHandler(IChat chat)
        {
            _chat = chat;
        }

        public void RegisterCommandHandlingStrategy(ILocalCommandHandlingStrategy strategy)
        {
            _cmdHandlingStrategyList.Add(strategy);
        }

        public void ClearCommandHandlingStrategies()
        {
            _cmdHandlingStrategyList.Clear();
        }

        public bool ExecuteCommand(string command)
        {
            bool executed = false;

            try
            {
                if (command.Length >= 2 && command.Substring(0, 2).Equals("//", StringComparison.InvariantCultureIgnoreCase))
                {
                    //COMMAND

                    command = command.Substring(2);

                    foreach (ILocalCommandHandlingStrategy strategy in _cmdHandlingStrategyList)
                    {
                        if (strategy.ValidateExecution(command))
                        {
                            executed = strategy.Execute();
                            if (executed)
                                break;
                        }
                    }

                    if (!executed)
                        _chat.UpdateLog($"Command [{command}] cannot be executed!");
                }
                else
                {
                    //CHAT MESSAGE
                    CommandHandler.Send(new ChatMessageCmdBuilder(command));
                }
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Local command execution error: {exception.Message}");
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
