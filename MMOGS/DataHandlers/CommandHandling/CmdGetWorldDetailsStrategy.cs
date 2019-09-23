using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Measurement.Units;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CmdGetWorldDetailsStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "getworlddetails";

        ILogger _logger;
        GameWorldData _gameWorldData;

        private string[] _cmdElements = null;
        private ClientCommandInfo _cmdInfo = null;

        public CmdGetWorldDetailsStrategy(ILogger logger, GameWorldData gameWorldData)
        {
            _logger = logger ?? throw new Exception("CmdGetWorldDetailsStrategy - logger cannot be NULL!");
            _gameWorldData = gameWorldData ?? throw new Exception("CmdGetWorldDetailsStrategy - game world data cannot be NULL!");
        }

        public bool ValidateExecution(ClientCommandInfo cmdInfo)
        {
            bool valid = false;
            _cmdInfo = cmdInfo;
            _cmdElements = cmdInfo.CommandTxt.Split(' ');

            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(_keyWord, GlobalData.InputDataStringComparison))
                valid = true;

            return valid;
        }

        public bool Execute(PlayerDetails playerDetails)
        {
            bool executed = false;
            int charId = playerDetails.CharId;
            int wmId = -1;
            CharacterData characterData = null;
            //CharacterData.CharacterState charState = CharacterData.CharacterState.Idle;
            bool isOnWorldMap = false;
            int parentObjectId = -1;
            CharacterPositionBasicDetails charPosBasicDetails = null;
            Point2<int> currentWorldLoc = null;
            Point3<double> currentLoc = null;
            WorldPlaceData worldPlace = null;
            PlaceInstance placeInstance = null;
            PlaceInstanceTerrainDetails[,,] placeInstanceTerrainDetails = null;
            List<WorldPlaceDataDetails> worldPlaceDataDetailsList = null;

            try
            {
                if (String.IsNullOrWhiteSpace(playerDetails.Login) || charId < 0)
                {
                    CommandHandler.Send(new InfoCmdBuilder("You're not logged in or your main character is not set!"), playerDetails);
                    throw new Exception("player not logged in or character not set!");
                }
                
                characterData = _gameWorldData.GetCharacterById(charId);
                if (characterData == null)
                    throw new Exception($"no character found (ID [{charId}])!");

                //charState = characterData.State;
                isOnWorldMap = characterData.IsOnWorldMap;
                wmId = characterData.WmId;
                parentObjectId = characterData.ParentObjectId;

                if (isOnWorldMap)
                {
                    currentWorldLoc = characterData.CurrentWorldLoc.Copy();
                }
                else
                {
                    characterData.GetLocationLocal(out currentLoc);
                }

                charPosBasicDetails = new CharacterPositionBasicDetails()
                {
                    CharId = charId,
                    WmId = wmId,
                    MapWidth = _gameWorldData.MapWidth,
                    MapHeight = _gameWorldData.MapHeight,
                    LocalBound = new Point3<int>
                    (
                        _gameWorldData.LocalBoundX, 
                        _gameWorldData.LocalBoundY, 
                        _gameWorldData.LocalBoundZ
                    ),
                    IsOnWorldMap = isOnWorldMap,
                    Position = new Point3<double>
                    (
                        (isOnWorldMap ? currentWorldLoc.X : currentLoc.X),
                        (isOnWorldMap ? currentWorldLoc.Y : currentLoc.Y),
                        (isOnWorldMap ? 0 : currentLoc.Z)
                    )
                };

                //BASIC DATA
                CommandHandler.Send(new CharPositionBasicDetailsCmdBuilder(charPosBasicDetails), playerDetails);

                //DETAILS
                if (!isOnWorldMap)
                {
                    worldPlace = _gameWorldData.GetWorldPlaceByWmId(wmId);
                    if (worldPlace == null)
                        throw new Exception($"cannot get world place with wm_id [{wmId}]");

                    placeInstance = worldPlace.GetPredefinedInstanceByParentObjectId(parentObjectId);
                    if (placeInstance == null)
                        throw new Exception($"cannot get place instance with parent obj. ID [{parentObjectId}] wm_id [{wmId}]");

                    placeInstanceTerrainDetails = placeInstance.GetTerrainDetails();
                    SendTerrainDetailsAsync(placeInstanceTerrainDetails, playerDetails);
                }
                else
                {
                    worldPlaceDataDetailsList = _gameWorldData.GetWorldPlaceDataDetails();
                    SendWorldPlaceDetailsAsync(worldPlaceDataDetailsList, playerDetails);
                }
                
                executed = true;
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Getting world data execution error for player with TCP client ID [{playerDetails.TcpClientId}]: {exception.Message}");
            }

            return executed;
        }

        private async void SendTerrainDetailsAsync(PlaceInstanceTerrainDetails[,,] placeInstanceTerrainDetails, PlayerDetails playerDetails)
        {
            await Task.Factory.StartNew(() => SendTerrainDetails(placeInstanceTerrainDetails, playerDetails));
        }

        private async void SendWorldPlaceDetailsAsync(List<WorldPlaceDataDetails> worldPlaceDataDetailsList, PlayerDetails playerDetails)
        {
            await Task.Factory.StartNew(() => SendWorldPlaceDetails(worldPlaceDataDetailsList, playerDetails));
        }

        private void SendTerrainDetails(PlaceInstanceTerrainDetails[,,] placeInstanceTerrainDetails, PlayerDetails playerDetails)
        {
            for (int z = 0; z < placeInstanceTerrainDetails.GetLength(2); z++)
            {
                for (int y = 0; y < placeInstanceTerrainDetails.GetLength(1); y++)
                {
                    for (int x = 0; x < placeInstanceTerrainDetails.GetLength(0); x++)
                    {
                        if (placeInstanceTerrainDetails[x, y, z] != null)
                            CommandHandler.Send(new TerrainDetailsCmdBuilder(false, placeInstanceTerrainDetails[x, y, z]), playerDetails);
                    }
                }
            }

            CommandHandler.Send(new TerrainDetailsCmdBuilder(true), playerDetails);
        }

        private void SendWorldPlaceDetails(List<WorldPlaceDataDetails> worldPlaceDataDetailsList, PlayerDetails playerDetails)
        {
            foreach (WorldPlaceDataDetails placeData in worldPlaceDataDetailsList)
            {
                if (placeData.WmId > -1)
                    CommandHandler.Send(new WorldDetailsCmdBuilder(false, placeData), playerDetails);
            }

            CommandHandler.Send(new WorldDetailsCmdBuilder(true), playerDetails);
        }
    }
}
