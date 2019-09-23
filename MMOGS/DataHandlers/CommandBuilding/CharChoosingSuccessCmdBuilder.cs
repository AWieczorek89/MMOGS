using MMOGS.Interfaces;
using MMOGS.Models.Commands;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class CharChoosingSuccessCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "charchoosingsuccess";

        private CommandDetails _commandDetails = new CommandDetails();
        private bool _choosingSuccess = false;

        public CharChoosingSuccessCmdBuilder(bool choosingSuccess)
        {
            _choosingSuccess = choosingSuccess;
        }

        public void AddKeyword()
        {
            if (_commandDetails.CommandElementList.Count > 0)
                throw new Exception($"CharChoosingSuccessCmdBuilder - cannot add key word to command. Command list is not empty, count [{_commandDetails.CommandElementList.Count}]!");

            _commandDetails.CommandElementList.Add(CharChoosingSuccessCmdBuilder._keyWord);
        }

        public void AddCommandElements()
        {
            if (_commandDetails.CommandElementList.Count != 1)
                throw new Exception($"CharChoosingSuccessCmdBuilder - cannot add command elements. Wrong count of key words [{_commandDetails.CommandElementList.Count}]!");

            _commandDetails.CommandElementList.Add(_choosingSuccess ? "true" : "false");
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
