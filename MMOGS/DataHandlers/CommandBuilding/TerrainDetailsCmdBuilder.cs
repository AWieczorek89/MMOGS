using MMOGS.Interfaces;
using MMOGS.Measurement.Units;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.Commands;
using MMOGS.Models.GameState;
using Newtonsoft.Json;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class TerrainDetailsCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "terrain";

        private CommandDetails _commandDetails = new CommandDetails();
        PlaceInstanceTerrainDetails _details = null;
        bool _isListConfirmation = false;

        public TerrainDetailsCmdBuilder(bool isListConfirmation, PlaceInstanceTerrainDetails details = null)
        {
            if (!isListConfirmation && details == null)
                throw new Exception("TerrainDetailsCmdBuilder - detail object cannot be NULL for no list confirmation!");

            _isListConfirmation = isListConfirmation;
            _details = details;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(TerrainDetailsCmdBuilder._keyWord);
        }

        public void AddCommandElements()
        {
            if (_isListConfirmation)
            {
                _commandDetails.CommandElementList.Add("endlist");
            }
            else
            {
                TerrainDetails detailsToSend = new TerrainDetails()
                {
                    ToId = _details.ToId,
                    LocalPos = new Point3<int>
                    (
                        _details.LocalPosX, 
                        _details.LocalPosY, 
                        _details.LocalPosZ
                    ),
                    ParentId = _details.ParentId,
                    IsParentalTeleport = _details.IsParentalTeleport,
                    IsExit = _details.IsExit,

                    TodId = _details.TerrainDefinition.TodId,
                    TodCode = _details.TerrainDefinition.Code,
                    TodCollision = new Point3<int>
                    (
                        _details.TerrainDefinition.CollisionX, 
                        _details.TerrainDefinition.CollisionY, 
                        _details.TerrainDefinition.CollisionZ
                    ),
                    TodIsTerrain = _details.TerrainDefinition.IsTerrain,
                    TodIsPlatform = _details.TerrainDefinition.IsPlatform,
                    TodIsObstacle = _details.TerrainDefinition.IsObstacle
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
