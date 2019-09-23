using MMOGS.Interfaces;
using MMOGS.Models.Commands;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class LoginSuccessCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "loginsuccess";

        private CommandDetails _commandDetails = new CommandDetails();
        private bool _loginSuccess = false;
        private string _info = "";

        public LoginSuccessCmdBuilder(bool loginSuccess, string info = "")
        {
            _loginSuccess = loginSuccess;
            _info = info;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(LoginSuccessCmdBuilder._keyWord);
        }

        public void AddCommandElements()
        {
            _commandDetails.CommandElementList.Add(_loginSuccess ? "true" : "false");

            if (!String.IsNullOrWhiteSpace(_info))
                _commandDetails.CommandElementList.Add(_info);
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
