using BackgroundManagement.Interfaces;
using BackgroundManagement.Measurement.Units;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using MMOC.BackgroundManagement;
using Newtonsoft.Json;
using System;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdCharPositionBasicDetailsStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "positionbasic";

        GameStateDetails _gameStateDetails;
        IChat _chat;
        private string[] _cmdElements;
        private string _rawText;

        public CmdCharPositionBasicDetailsStrategy(IChat chat, GameStateDetails gameStateDetails)
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
                string jsonString = _rawText.Substring(_keyWord.Length);
                CharacterPositionBasicDetails charPosDetails = JsonConvert.DeserializeObject<CharacterPositionBasicDetails>(jsonString);

                _gameStateDetails.CharId = charPosDetails.CharId;
                _gameStateDetails.WmId = charPosDetails.WmId;
                _gameStateDetails.MapWidth = charPosDetails.MapWidth;
                _gameStateDetails.MapHeight = charPosDetails.MapHeight;
                _gameStateDetails.LocalBound = PointConverter.Point3ToVector(charPosDetails.LocalBound);
                _gameStateDetails.IsOnWorldMap = charPosDetails.IsOnWorldMap;
                _gameStateDetails.Position = PointConverter.Point3ToVector(charPosDetails.Position);

                //_gameStateDetails.LogCurrentState(); //for testing

                if (charPosDetails.IsOnWorldMap)
                {
                    MainGameHandler.ChangeScene(MainGameHandler.SceneType.WorldMap);
                }
                else
                {
                    MainGameHandler.ChangeScene(MainGameHandler.SceneType.LocalPlace);
                }

                executed = true;
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Cannot execute char. basic details command: {exception.Message}");
            }

            return executed;
        }
    }
}
