using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CmdGetLocationCharsStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "getlocalchars";

        ILogger _logger;
        ICharacterInfo _characterInfo;

        private string[] _cmdElements = null;
        private ClientCommandInfo _cmdInfo = null;

        private readonly object _dataLock = new object();
        
        public CmdGetLocationCharsStrategy(ILogger logger, ICharacterInfo characterInfo)
        {
            _logger = logger ?? throw new Exception("CmdGetLocationCharsStrategy - logger cannot be NULL!");
            _characterInfo = characterInfo ?? throw new Exception("CmdGetLocationCharsStrategy - character info cannot be NULL!");
        }

        public bool ValidateExecution(ClientCommandInfo cmdInfo)
        {
            bool valid = false;

            lock (_dataLock)
            {
                if
                (
                    cmdInfo.CommandTxt.Length >= _keyWord.Length &&
                    cmdInfo.CommandTxt.Substring(0, _keyWord.Length).Equals(_keyWord, GlobalData.InputDataStringComparison)
                )
                {
                    _cmdInfo = cmdInfo;
                    _cmdElements = cmdInfo.CommandTxt.Split(' ');
                    valid = true;
                }
            }
            
            return valid;
        }
        
        public bool Execute(PlayerDetails playerDetails)
        {
            if (String.IsNullOrWhiteSpace(playerDetails.Login) || playerDetails.CharId < 0)
            {
                CommandHandler.Send(new InfoCmdBuilder("Access denied!"), playerDetails);
                _logger.UpdateLog($"Location characters getting method cannot be executed - empty login or char. ID less than 0! For TCP client ID [{playerDetails.TcpClientId}]");
                return false;
            }

            ExecuteAsync(playerDetails);
            Thread.Sleep(5);
            return true;
        }

        private async void ExecuteAsync(PlayerDetails playerDetails)
        {
            await Task.Factory.StartNew
            (
                () =>
                {
                    lock (_dataLock)
                    {
                        try
                        {
                            if (_cmdElements.Length < 1 || !_cmdElements[0].Equals(_keyWord, GlobalData.InputDataStringComparison))
                            {
                                throw new Exception($"command replacement error [{_cmdInfo.CommandTxt}]");
                            }
                            else
                            {
                                CharacterData mainCharacter = _characterInfo.GetCharacterById(playerDetails.CharId);
                                if (mainCharacter == null)
                                    throw new Exception($"cannot get main character with char_id [{playerDetails.CharId}]");

                                int wmId = mainCharacter.WmId;
                                int parentObjectId = mainCharacter.ParentObjectId;
                                bool isOnWorldMap = mainCharacter.IsOnWorldMap;

                                int charIdFilter = -1;
                                if (_cmdElements.Length == 2 && !String.IsNullOrWhiteSpace(_cmdElements[1]))
                                {
                                    Int32.TryParse(_cmdElements[1], out charIdFilter);
                                }

                                List<CharacterData> charsList = _characterInfo.GetCharactersByWorldLocation(wmId, isOnWorldMap, parentObjectId);

                                if (charIdFilter < 0)
                                {
                                    foreach (CharacterData character in charsList)
                                    {
                                        CommandHandler.Send(new LocalCharacterDetailsCmdBuilder(false, character), playerDetails);
                                    }

                                    CommandHandler.Send(new LocalCharacterDetailsCmdBuilder(true), playerDetails);
                                }
                                else
                                {
                                    foreach (CharacterData character in charsList)
                                    {
                                        if (character.CharId == charIdFilter)
                                        {
                                            CommandHandler.Send
                                            (
                                                new LocalCharacterDetailsCmdBuilder(false, character, LocalCharacterDetailsCmdBuilder.Action.OnRequestSpawn),
                                                playerDetails
                                            );
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            CommandHandler.Send(new InfoCmdBuilder("Internal server error (cannot get local characters)!"), playerDetails);
                            _logger.UpdateLog($"Location characters getting error for TCP client ID [{playerDetails.TcpClientId}]: {exception.Message}");
                        }
                    }
                }
            );
        }
    }
}
