using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using MMOC.BackgroundManagement;
using Newtonsoft.Json;
using System;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdLocalCharacterDetailsStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "localchar";

        private GameStateDetails _gameStateDetails;
        private IChat _chat;
        private string[] _cmdElements;
        private string _rawText;

        public CmdLocalCharacterDetailsStrategy(IChat chat, GameStateDetails gameStateDetails)
        {
            _chat = chat;
            _gameStateDetails = gameStateDetails;
        }

        public bool ValidateExecution(string command)
        {
            bool valid = false;

            _cmdElements = command.Split(' ');
            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(_keyWord, GlobalData.InputDataStringComparison))
            {
                valid = true;
                _rawText = command;
            }

            return valid;
        }

        public bool Execute()
        {
            bool executed = false;

            try
            {
                if (_cmdElements.Length < 2)
                    throw new Exception("wrong count of elements");
                
                if (_cmdElements[1].Equals("position", GlobalData.InputDataStringComparison))
                {
                    string jsonTxt = _rawText.Substring($"{_keyWord} position".Length);
                    LocalCharacterDetails details = JsonConvert.DeserializeObject<LocalCharacterDetails>(jsonTxt);
                    _gameStateDetails.AddOrUpdateLocalCharacterDetails(details);
                    
                    if (details.Action.Equals("requestspawn"))
                        LocalPlaceSceneManagerHandler.AddNewNonPlayerCharacterExternally(details);
                }
                else
                if (_cmdElements[1].Equals("endlist", GlobalData.InputDataStringComparison))
                {
                    _gameStateDetails.LocalCharacterDetailsListConfirmed = true;
                }
                else
                {
                    throw new Exception($"wrong second keyword [{_cmdElements[1]}]");
                }
                
                executed = true;
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Cannot execute local character details command: {exception.Message}");
            }

            return executed;
        }
    }
}
