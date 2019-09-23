using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using BackgroundManagement.Interfaces;
using System;

namespace BackgroundManagement.DataHandlers.LocalCommandHandling
{
    public class LocalCmdSwitchPlaceStrategy : ILocalCommandHandlingStrategy
    {
        private static readonly string _keyWord = "switchplace";

        public bool ValidateExecution(string command)
        {
            return command.Equals(_keyWord, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Execute()
        {
            CommandHandler.Send(new SwitchPlaceRequestCmdBuilder());
            return true;
        }
    }
}
