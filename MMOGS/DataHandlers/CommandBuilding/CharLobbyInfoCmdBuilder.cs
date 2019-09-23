using MMOGS.Interfaces;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.Commands;
using MMOGS.Models.Database;
using MMOGS.Models.GameState;
using Newtonsoft.Json;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class CharLobbyInfoCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "lobbychar";

        private CommandDetails _commandDetails = new CommandDetails();
        private CharacterData _characterData;
        private bool _isListConfirmation = false;

        public CharLobbyInfoCmdBuilder(bool isListConfirmation, CharacterData characterData = null)
        {
            _isListConfirmation = isListConfirmation;
            _characterData = characterData;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(CharLobbyInfoCmdBuilder._keyWord);
        }

        public void AddCommandElements()
        {
            if (_isListConfirmation)
            {
                _commandDetails.CommandElementList.Add("endlist");
            }
            else
            {
                DbCharactersData dbData = _characterData.GetDbData();
                LobbyCharDetails charDetails = new LobbyCharDetails()
                {
                    CharId = dbData.CharId,
                    Name = dbData.Name,
                    ModelCode = dbData.ModelCode,
                    HairstyleId = dbData.HairstyleId
                };

                _commandDetails.CommandElementList.Add("position");
                _commandDetails.CommandElementList.Add(JsonConvert.SerializeObject(charDetails));
            }
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
