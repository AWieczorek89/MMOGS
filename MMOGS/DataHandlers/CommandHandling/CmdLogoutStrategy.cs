using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Models.GameState;
using System;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CmdLogoutStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "logout";

        PlayerHandler _playerHandler;

        public CmdLogoutStrategy(PlayerHandler playerHandler)
        {
            _playerHandler = playerHandler ?? throw new Exception("CmdLogoutStrategy - player handler cannot be NULL!");
        }

        public bool ValidateExecution(ClientCommandInfo cmdInfo)
        {
            return (cmdInfo.CommandTxt.Equals(_keyWord, GlobalData.InputDataStringComparison));
        }

        public bool Execute(PlayerDetails playerDetails)
        {
            ExecuteAsync(playerDetails);
            return true;
        }

        public async void ExecuteAsync(PlayerDetails playerDetails)
        {
            CommandHandler.Send(new InfoCmdBuilder("you're successfully logged out!"), playerDetails);
            await Task.Factory.StartNew(() => Thread.Sleep(1000));
            TcpServer.KickClient(playerDetails.TcpClientId);
            _playerHandler.UnregisterPlayer(playerDetails.TcpClientId);
        }
    }
}
