using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using MMOC.BackgroundManagement;
using Newtonsoft.Json;
using System;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdWorldPlaceDetailsStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "worldplace";

        private GameStateDetails _gameStateDetails;
        private IChat _chat;
        private string[] _cmdElements;
        private string _rawText;

        public CmdWorldPlaceDetailsStrategy(IChat chat, GameStateDetails gameStateDetails)
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
                    WorldPlaceDataDetails details = JsonConvert.DeserializeObject<WorldPlaceDataDetails>(jsonTxt);
                    _gameStateDetails.AddOrUpdateWorldPlaceDetails(details);
                }
                else
                if (_cmdElements[1].Equals("endlist", GlobalData.InputDataStringComparison))
                {
                    _gameStateDetails.WorldPlaceDetailsListConfirmed = true;
                }
                else
                {
                    throw new Exception($"wrong second keyword [{_cmdElements[1]}]");
                }

                executed = true;
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Cannot execute world place details command: {exception.Message}");
            }

            return executed;
        }
    }
}
