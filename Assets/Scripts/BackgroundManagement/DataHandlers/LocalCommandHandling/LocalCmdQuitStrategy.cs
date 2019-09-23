using Assets.Scripts.BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using BackgroundManagement.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundManagement.DataHandlers.LocalCommandHandling
{
    public class LocalCmdQuitStrategy : ILocalCommandHandlingStrategy
    {
        private static readonly string _keyWord = "quit";

        public bool ValidateExecution(string command)
        {
            return command.Equals(_keyWord, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Execute()
        {
            ExecuteAsync();
            return true;
        }

        private async void ExecuteAsync()
        {
            await Task.Factory.StartNew(() => Thread.Sleep(500));
            CommandHandler.Send(new LogoutCmdBuilder());
            MainGameHandler.CloseApplication();
        }
    }
}
