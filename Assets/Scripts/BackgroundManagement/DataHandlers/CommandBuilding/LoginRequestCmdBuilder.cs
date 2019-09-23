using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.Commands;
using System;

namespace BackgroundManagement.DataHandlers.CommandBuilding
{
    public class LoginRequestCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "login";

        private CommandDetails _commandDetails = new CommandDetails();
        private string _login = "";
        private string _pass = "";

        public LoginRequestCmdBuilder(string login, string pass)
        {
            _login = login;
            _pass = pass;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(_keyWord);
        }

        public void AddCommandElements()
        {
            _commandDetails.CommandElementList.Add(_login);

            if (!String.IsNullOrEmpty(_pass))
                _commandDetails.CommandElementList.Add(_pass);
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
