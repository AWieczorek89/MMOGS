using MMOGS.Interfaces;
using MMOGS.Models.Commands;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class ServerTimeCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "servertime";

        private CommandDetails _commandDetails = new CommandDetails();

        public ServerTimeCmdBuilder()
        {
        }

        public void AddCommandElements()
        {
            _commandDetails.CommandElementList.Add(DateTime.Now.ToString("HH-mm-ss"));
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(ServerTimeCmdBuilder._keyWord);
        }

        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
