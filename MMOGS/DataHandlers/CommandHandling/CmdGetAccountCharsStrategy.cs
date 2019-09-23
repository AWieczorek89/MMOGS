using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Models.Database;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using TcpConnector;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CmdGetAccountCharsStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "getaccountchars";

        ILogger _logger;
        AccountData _accountData;
        ICharacterInfo _characterInfo;

        private string[] _cmdElements = null;
        private ClientCommandInfo _cmdInfo = null;

        public CmdGetAccountCharsStrategy(ILogger logger, AccountData accountData, ICharacterInfo characterInfo)
        {
            _logger = logger ?? throw new Exception("CmdGetAccountCharsStrategy - logger cannot be NULL!");
            _accountData = accountData ?? throw new Exception("CmdGetAccountCharsStrategy - account data cannot be NULL!");
            _characterInfo = characterInfo ?? throw new Exception("CmdGetAccountCharsStrategy - character info cannot be NULL!");
        }

        public bool ValidateExecution(ClientCommandInfo cmdInfo)
        {
            bool valid = false;
            _cmdInfo = cmdInfo;
            _cmdElements = cmdInfo.CommandTxt.Split(' ');

            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(_keyWord , GlobalData.InputDataStringComparison))
                valid = true;

            return valid;
        }

        public bool Execute(PlayerDetails playerDetails)
        {
            bool executed = false;
            DbAccountsData account = null;
            List<CharacterData> charsList = null;

            try
            {
                if (String.IsNullOrWhiteSpace(playerDetails.Login))
                {
                    CommandHandler.Send(new InfoCmdBuilder("You're not logged in!"), playerDetails);
                    throw new Exception("player is not logged in!");
                }

                if (playerDetails.CharId > -1)
                {
                    playerDetails.CharId = -1;
                    _logger.UpdateLog($"Player's char ID set to -1 by executing char list method for TCP client ID [{playerDetails.TcpClientId}]");
                }

                account = _accountData.GetAccountData(playerDetails.Login);
                if (account == null)
                {
                    CommandHandler.Send(new InfoCmdBuilder("An error occured on the server."), playerDetails);
                    throw new Exception("cannot get db account data (NULL)!");
                }

                charsList = _characterInfo.GetCharactersByAccId(account.AccId);

                foreach (CharacterData charData in charsList)
                {
                    //LOBBY CHARACTER DETAILS
                    CommandHandler.Send(new CharLobbyInfoCmdBuilder(false, charData), playerDetails);
                }

                //LIST CONFIRMATION
                CommandHandler.Send(new CharLobbyInfoCmdBuilder(true), playerDetails);

                executed = true;
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Account characters getting error for TCP client ID [{playerDetails.TcpClientId}]: {exception.Message}");
            }

            return executed;
        }
    }
}
