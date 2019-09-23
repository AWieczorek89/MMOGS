using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Measurement;
using MMOGS.Measurement.Units;
using MMOGS.Models;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class SwitchPlaceRequestStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "switchplace";

        private ILogger _logger;
        private ICharacterActionManager _charActionManager;
        private ICharacterInfo _charInfo;
        private IGeoDataInfo _geoDataInfo;
        private PlayerHandler _playerHandler;

        private string[] _cmdElements = null;
        private ClientCommandInfo _cmdInfo = null;

        public SwitchPlaceRequestStrategy(ILogger logger, ICharacterActionManager charActionManager, ICharacterInfo charInfo, IGeoDataInfo geoDataInfo, PlayerHandler playerHandler)
        {
            _logger = logger ?? throw new Exception("SwitchPlaceRequestStrategy - logger cannot be NULL!");
            _charActionManager = charActionManager ?? throw new Exception("SwitchPlaceRequestStrategy - char. action manager cannot be NULL!");
            _charInfo = charInfo ?? throw new Exception("SwitchPlaceRequestStrategy - char. info cannot be NULL!");
            _playerHandler = playerHandler ?? throw new Exception("SwitchPlaceRequestStrategy - player handler cannot be NULL!");
            _geoDataInfo = geoDataInfo ?? throw new Exception("SwitchPlaceRequestStrategy - geo data info cannot be NULL!");
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
            ExecuteAsync(playerDetails);
            return true;
        }

        private async void ExecuteAsync(PlayerDetails playerDetails)
        {
            try
            {
                int charId = playerDetails.CharId;
                if (charId < 0)
                    throw new Exception("char. ID not set!");

                CharacterData charData = await _charInfo.GetCharacterByIdTaskStart(charId);
                
                if (charData.IsOnWorldMap)
                {
                    _charActionManager.SwitchCharacterMovementType(charId);
                }
                else
                {
                    int wmId = charData.WmId;
                    int parentObjectId = charData.ParentObjectId;

                    Point3<int> pointFrom = new Point3<int>
                    (
                        Convert.ToInt32(charData.CurrentLoc.X),
                        Convert.ToInt32(charData.CurrentLoc.Y),
                        Convert.ToInt32(charData.CurrentLoc.Z) - 1
                    );

                    Point3<int> pointTo = new Point3<int>
                    (
                        Convert.ToInt32(charData.CurrentLoc.X),
                        Convert.ToInt32(charData.CurrentLoc.Y),
                        Convert.ToInt32(charData.CurrentLoc.Z) + 1
                    );

                    using (BoxedData boxedGeoData = await _geoDataInfo.GetLocalGeoDataTaskStart(wmId, parentObjectId, pointFrom, pointTo))
                    {
                        List<GeoDataElement> geoDataElementList = (List<GeoDataElement>)boxedGeoData.Data;
                        if (!String.IsNullOrEmpty(boxedGeoData.Msg))
                            _logger.UpdateLog(boxedGeoData.Msg);

                        bool exitFound = false;
                        int collisionMinX, collisionMaxX, collisionMinY, collisionMaxY, collisionMinZ, collisionMaxZ;

                        foreach (GeoDataElement geoElement in geoDataElementList)
                        {
                            if (!geoElement.IsExit)
                                continue;

                            GeoDataValidator.GetColliderPosition(geoElement, out collisionMinX, out collisionMaxX, out collisionMinY, out collisionMaxY, out collisionMinZ, out collisionMaxZ);

                            //_logger.UpdateLog($"exit col. x [{collisionMinX}-{collisionMaxX}] y [{collisionMinY}-{collisionMaxY}] z [{collisionMinZ}-{collisionMaxZ}]");
                            //_logger.UpdateLog($"current [{charData.CurrentLoc.X}; {charData.CurrentLoc.Y}; {charData.CurrentLoc.Z}]");

                            if
                            (
                                charData.CurrentLoc.X >= collisionMinX && charData.CurrentLoc.X <= collisionMaxX + 1 &&
                                charData.CurrentLoc.Y >= collisionMinY && charData.CurrentLoc.Y <= collisionMaxY + 1 &&
                                charData.CurrentLoc.Z >= collisionMinZ && charData.CurrentLoc.Z <= collisionMaxZ + 2
                            )
                            {
                                exitFound = true;
                                break;
                            }
                        }

                        if (exitFound)
                        {
                            _charActionManager.SwitchCharacterMovementType(charId);
                        }
                        else
                        {
                            CommandHandler.Send(new InfoCmdBuilder("You're not at exit point!"), playerDetails);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Switch place command execution error: {exception.Message}");
            }
        }
    }
}
