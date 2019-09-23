using BackgroundManagement.Interfaces;
using MMOC.BackgroundManagement;
using System;
using System.Globalization;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdServerTimeStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "servertime";

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

            if (_cmdElements.Length == 2 && !String.IsNullOrWhiteSpace(_cmdElements[1]))
            {
                DateTime result;

                if (DateTime.TryParseExact(_cmdElements[1], "HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    ConnectionChecker checker = MainGameHandler.GetConnectionChecker();
                    if (checker != null)
                    {
                        checker.ServerTime = result;
                        executed = true;
                    }
                }
            }
            return executed;
        }
    }
}
