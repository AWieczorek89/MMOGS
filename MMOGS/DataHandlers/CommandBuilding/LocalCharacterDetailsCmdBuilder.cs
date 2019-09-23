using MMOGS.Interfaces;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.Commands;
using MMOGS.Models.GameState;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class LocalCharacterDetailsCmdBuilder : ICommandBuilder
    {
        public enum Action
        {
            OnSceneStartSpawn,
            OnRequestSpawn
        }

        private static Dictionary<Action, string> _actionDictionary = new Dictionary<Action, string>()
        {
            { Action.OnSceneStartSpawn, "startspawn" },
            { Action.OnRequestSpawn, "requestspawn" }
        };
        
        private static readonly string _keyWord = "localchar";

        private CommandDetails _commandDetails = new CommandDetails();
        CharacterData _charData = null;
        bool _isListConfirmation = false;
        Action _action;

        public LocalCharacterDetailsCmdBuilder(bool isListConfirmation, CharacterData charData = null, Action action = Action.OnSceneStartSpawn)
        {
            if (!isListConfirmation && charData == null)
                throw new Exception("LocalCharacterDetailsCmdBuilder - character data cannot be NULL for no list confirmation!");

            _isListConfirmation = isListConfirmation;
            _charData = charData;
            _action = action;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(_keyWord);
        }

        public void AddCommandElements()
        {
            if (_isListConfirmation)
            {
                _commandDetails.CommandElementList.Add("endlist");
            }
            else
            {
                string state;
                string actionString = "ndef";
                _actionDictionary.TryGetValue(_action, out actionString);
                
                switch (_charData.State)
                {
                    case CharacterData.CharacterState.Moving:
                        state = "moving";
                        break;
                    default:
                        state = "idle";
                        break;
                }
                
                LocalCharacterDetails detailsToSend = new LocalCharacterDetails()
                {
                    Name = _charData.Name,
                    HairstyleId = _charData.HairstyleId,
                    CharId = _charData.CharId,
                    Action = actionString,
                    State = state,
                    Angle = _charData.Angle,
                    Velocity = _charData.Velocity,
                    MovingStartTime = _charData.MovingStartTime.ToString(GlobalData.MovementTimeExchangeFormat),
                    MovingEndTime = _charData.MovingEndTime.ToString(GlobalData.MovementTimeExchangeFormat),
                    StartingLoc = _charData.StartingLoc,
                    DestinationLoc = _charData.DestinationLoc,
                    CurrentLoc = _charData.CurrentLoc,
                    CurrentWorldLoc = _charData.CurrentWorldLoc,
                    ModelCode = _charData.ModelCode
                };

                _commandDetails.CommandElementList.Add("position");
                _commandDetails.CommandElementList.Add(JsonConvert.SerializeObject(detailsToSend));
            }
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
