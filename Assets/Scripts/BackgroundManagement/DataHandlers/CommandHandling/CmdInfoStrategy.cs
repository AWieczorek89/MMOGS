using BackgroundManagement.Interfaces;
using MMOC.BackgroundManagement;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdInfoStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "info";
        private static string _content = "";
        
        public bool ValidateExecution(string command)
        {
            bool valid = false;
            
            if (command.Length >= _keyWord.Length)
            {
                valid = (command.Substring(0, _keyWord.Length).Equals(_keyWord, GlobalData.InputDataStringComparison));
                if (valid)
                    _content = command.Substring(_keyWord.Length);
            }

            return valid;
        }

        public bool Execute()
        {
            MainGameHandler.ShowMessageBox($"Server info: {_content}", "Server message", null);
            return true;
        }
    }
}
