using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Models.Database;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CmdChooseCharacterStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "choosechar";

        private ILogger _logger;
        private AccountData _accountData;
        private ICharacterInfo _characterInfo;
        private PlayerHandler _playerHandler;

        private string[] _cmdElements = null;
        private ClientCommandInfo _cmdInfo = null;

        public CmdChooseCharacterStrategy(ILogger logger, AccountData accountData, ICharacterInfo characterInfo, PlayerHandler playerHandler)
        {
            _logger = logger ?? throw new Exception("CmdChooseCharacterStrategy - logger cannot be NULL!");
            _accountData = accountData ?? throw new Exception("CmdChooseCharacterStrategy - account data cannot be NULL!");
            _characterInfo = characterInfo ?? throw new Exception("CmdChooseCharacterStrategy - character info cannot be NULL!");
            _playerHandler = playerHandler ?? throw new Exception("CmdChooseCharacterStrategy - player handler cannot be NULL!");
        }

        public bool ValidateExecution(ClientCommandInfo cmdInfo)
        {
            bool valid = false;
            _cmdInfo = cmdInfo;
            _cmdElements = cmdInfo.CommandTxt.Split(' ');

            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(CmdChooseCharacterStrategy._keyWord, GlobalData.InputDataStringComparison))
                valid = true;

            return valid;
        }

        public bool Execute(PlayerDetails playerDetails)
        {
            bool executed = false;
            DbAccountsData account = null;
            List<CharacterData> charsList = null;
            int charId = -1;

            try
            {
                if (String.IsNullOrWhiteSpace(playerDetails.Login))
                {
                    CommandHandler.Send(new InfoCmdBuilder("You're not logged in!"), playerDetails);
                    throw new Exception("player is not logged in!");
                }

                if (_cmdElements.Length != 2)
                {
                    CommandHandler.Send(new InfoCmdBuilder("Character choosing - wrong command!"), playerDetails);
                    throw new Exception($"wrong command element count [{_cmdElements.Length}]");
                }

                if (String.IsNullOrWhiteSpace(_cmdElements[1]) || !Int32.TryParse(_cmdElements[1], out charId))
                {
                    CommandHandler.Send(new InfoCmdBuilder("Character choosing - wrong ID, must be numeric value!"), playerDetails);
                    throw new Exception($"cannot convert character ID from element [{_cmdElements[1]}]");
                }

                if (charId < 0)
                {
                    CommandHandler.Send(new InfoCmdBuilder("Character choosing - wrong ID, cannot be less than 0!"), playerDetails);
                    throw new Exception($"wrong character ID [{charId}] (less than 0)");
                }

                account = _accountData.GetAccountData(playerDetails.Login);

                if (_playerHandler.GetPlayerCharacterId(account.Login) > -1)
                {
                    CommandHandler.Send(new InfoCmdBuilder("You have already chosen your character (character in use)!"), playerDetails);
                    throw new Exception("other character in use!");
                }
                
                if (account == null)
                {
                    CommandHandler.Send(new InfoCmdBuilder("An error occured on the server."), playerDetails);
                    throw new Exception("cannot get db account data (NULL)!");
                }

                charsList = _characterInfo.GetCharactersByAccId(account.AccId);

                foreach (CharacterData charData in charsList)
                {
                    if (charData.CharId == charId)
                    {
                        _playerHandler.SetPlayerCharacter(charId, account.Login);
                        _logger.UpdateLog($"Character set to char. ID [{charId}] for player [{account.Login}]");
                        executed = true;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Character choosing execution error for player with TCP client ID [{playerDetails.TcpClientId}]: {exception.Message}");
            }
            finally
            {
                //CHARACTER CHOOSING RESULT
                CommandHandler.Send(new CharChoosingSuccessCmdBuilder(executed), playerDetails);
            }

            return executed;
        }
    }
}
