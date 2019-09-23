using BackgroundManagement.Interfaces;
using MMOC.BackgroundManagement;
using System;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdPingResponseStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "pingres";

        private string[] _cmdElements = null;

        public bool ValidateExecution(string command)
        {
            bool valid = false;
            _cmdElements = command.Split(' ');

            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(CmdPingResponseStrategy._keyWord, GlobalData.InputDataStringComparison))
                valid = true;

            return valid;
        }

        public bool Execute()
        {
            bool executed = false;
            int pingId = -1;

            if (_cmdElements.Length == 2)
            {
                if (Int32.TryParse(_cmdElements[1], out pingId))
                {
                    ConnectionChecker.SetPingResponse(pingId, DateTime.Now);
                    executed = true;
                }
            }

            return executed;
        }
    }
}
