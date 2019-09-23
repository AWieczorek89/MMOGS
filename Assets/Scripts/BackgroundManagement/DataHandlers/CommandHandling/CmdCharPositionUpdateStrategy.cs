using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.Interfaces;
using BackgroundManagement.Measurement.Units;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using MMOC.BackgroundManagement;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdCharPositionUpdateStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "charposupdate";

        private IChat _chat;
        private GameStateDetails _gameStateDetails;
        private string[] _cmdElements;
        private string _rawText;

        public CmdCharPositionUpdateStrategy(IChat chat, GameStateDetails gameStateDetails)
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

            //Debug.Log(_rawText);

            try
            {
                string jsonText = _rawText.Substring(_keyWord.Length);
                CharacterPositionUpdateDetails posUpdateDetails = JsonConvert.DeserializeObject<CharacterPositionUpdateDetails>(jsonText);
                int currentCharId = _gameStateDetails.CharId;
                bool isOnWorldMap = _gameStateDetails.IsOnWorldMap;

                if 
                (
                    posUpdateDetails.MovementType.Equals("movelocal", GlobalData.InputDataStringComparison) || 
                    posUpdateDetails.MovementType.Equals("moveworld", GlobalData.InputDataStringComparison)
                )
                {
                    #region Movement section
                    //MOVEMENT SECTION

                    if (isOnWorldMap)
                    {
                        if (currentCharId == posUpdateDetails.CharId)
                        {
                            Point2<int> oldLocationWorld = posUpdateDetails.OldLocationWorld;
                            Point2<int> newLocationWorld = posUpdateDetails.NewLocationWorld;

                            _gameStateDetails.Position = new Vector3
                            (
                                newLocationWorld.X,
                                newLocationWorld.Y,
                                0f
                            );

                            WorldMapSceneManagerHandler.MovePlayerExternally
                            (
                                PointConverter.Point2ToVector(oldLocationWorld),
                                PointConverter.Point2ToVector(newLocationWorld)
                            );
                        }
                    }
                    else
                    {
                        LocalPlaceSceneManagerHandler.MovePlayerExternally
                        (
                            posUpdateDetails.CharId,
                            /*posUpdateDetails.OldLocationLocal, */
                            posUpdateDetails.NewLocationLocal,
                            posUpdateDetails.TimeArrivalMsLocal
                        );
                    }

                    #endregion
                }
                else
                if (posUpdateDetails.MovementType.Equals("switchmap", GlobalData.InputDataStringComparison))
                {
                    if (currentCharId == posUpdateDetails.CharId)
                    {
                        SwitchScene();
                    }
                    else
                    {
                        LocalPlaceSceneManagerHandler.RemoveNonPlayerCharacterExternally(posUpdateDetails.CharId);
                    }
                }
                else
                if (posUpdateDetails.MovementType.Equals("switchlocal", GlobalData.InputDataStringComparison))
                {
                    if (currentCharId == posUpdateDetails.CharId)
                    {
                        SwitchScene();
                    }
                    else
                    {
                        CommandHandler.Send(new GetLocationCharsCmdBuilder(posUpdateDetails.CharId));
                    }
                }
                else
                {
                    throw new Exception($"unknown movement type [{posUpdateDetails.MovementType}]");
                }

                executed = true;
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Character position update command execution error: {exception.Message}");
            }

            return executed;
        }

        private void SwitchScene()
        {
            MainGameHandler.ShowLoadingScreen();
            MainGameHandler.GetGameStateDetails().Reset();
            CommandHandler.Send(new GetWorldDetailsCmdBuilder());
        }
    }
}
