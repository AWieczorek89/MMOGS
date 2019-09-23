using BackgroundManagement.Interfaces;
using MMOC.BackgroundManagement;
using System;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdLoginSuccessStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "loginsuccess";
        
        private string[] _cmdElements;

        public bool ValidateExecution(string command)
        {
            bool valid = false;
            _cmdElements = command.Split(' ');

            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(_keyWord, GlobalData.InputDataStringComparison))
                valid = true;

            return valid;
        }

        public bool Execute()
        {
            bool executed = false;

            try
            {
                bool loginSuccess = false;
                string info = "";
                ConnectionChecker connChecker = MainGameHandler.GetConnectionChecker();

                if (_cmdElements.Length >= 2)
                    loginSuccess = _cmdElements[1].Equals("true", GlobalData.InputDataStringComparison);

                if (_cmdElements.Length > 2)
                {
                    for (int i = 2; i < _cmdElements.Length; i++)
                    {
                        if (i > 2) info += ' ';
                        info += _cmdElements[i];
                    }
                }

                connChecker.SetLoginState(loginSuccess ? ConnectionChecker.LoginState.Logged : ConnectionChecker.LoginState.NotLoggedInOrLoginFailed);

                if (info.Length > 0)
                    MainGameHandler.ShowMessageBox(info, "Server login message", null);

                executed = true;
            }
            catch (Exception exception)
            {
                MainGameHandler.ShowMessageBox($"Cannot execute login command: {exception.Message}", "Login error", null);
            }
            
            return executed;
        }
    }
}
