using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Models.GameState;
using System;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CmdPingStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "ping";

        private string[] _cmdElements = null;
        private ClientCommandInfo _cmdInfo = null;

        public bool ValidateExecution(ClientCommandInfo cmdInfo)
        {
            bool valid = false;
            _cmdInfo = cmdInfo;
            _cmdElements = cmdInfo.CommandTxt.Split(' ');

            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(_keyWord, GlobalData.InputDataStringComparison))
                valid = true;

            return valid;
        }

        public bool Execute(PlayerDetails playerDetails)
        {
            bool executed = false;
            int pingId = -1;

            if (_cmdElements.Length == 2)
            {
                if (Int32.TryParse(_cmdElements[1], out pingId))
                {
                    CommandHandler.Send(new PingResponseCmdBuilder(pingId), playerDetails);
                    executed = true;
                }
            }

            return executed;
        }
    }
}
