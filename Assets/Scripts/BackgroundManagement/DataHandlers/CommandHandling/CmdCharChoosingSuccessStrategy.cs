using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.Interfaces;
using MMOC.BackgroundManagement;
using System;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdCharChoosingSuccessStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "charchoosingsuccess";
        
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
                bool charChoosingSuccess = false;

                if (_cmdElements.Length == 2)
                {
                    if (_cmdElements[1].Equals("true", GlobalData.InputDataStringComparison))
                        charChoosingSuccess = true;
                }

                if (charChoosingSuccess)
                {
                    MainGameHandler.ShowLoadingScreen();
                    MainGameHandler.GetGameStateDetails().Reset();
                    CommandHandler.Send(new GetWorldDetailsCmdBuilder());
                }
                else
                {
                    MainGameHandler.ShowMessageBox("Server declined character selection!");
                }

                executed = true;
            }
            catch (Exception exception)
            {
                MainGameHandler.ShowMessageBox($"Cannot execute character choosing success command! {exception.Message}");
            }

            return executed;
        }
    }
}
