using MMOGS.Interfaces;
using MMOGS.Models.Commands;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class InfoCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "info";
        
        private CommandDetails _commandDetails = new CommandDetails();
        private string _info = "";

        public InfoCmdBuilder(string info)
        {
            _info = info;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(_keyWord);
        }

        public void AddCommandElements()
        {
            _commandDetails.CommandElementList.Add(_info);
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
