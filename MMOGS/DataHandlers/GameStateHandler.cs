using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.DataHandlers.CommandHandling;
using MMOGS.Interfaces;
using MMOGS.Measurement;
using MMOGS.Measurement.Units;
using MMOGS.Models;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.GameState;

namespace MMOGS.DataHandlers
{
    public class GameStateHandler : IDisposable, ICharacterActionManager
    {
        private ILogger _logger = null;
        private IGeoDataInfo _geoDataInfo = null;
        private ICharacterInfo _characterInfo = null;
        private PlayerHandler _playerHandler = null;

        private bool _sendingMovementDetailsInProgress = false;
        public bool SendingMovementDetailsInProgress
        {
            get { lock (_sendingFlagLock) { return _sendingMovementDetailsInProgress; } }
            private set { lock (_sendingFlagLock) { _sendingMovementDetailsInProgress = value; } }
        }

        private bool _movementHandlingInProgress = false;
        private List<CharacterMovementDetails> _movementDetailsList = new List<CharacterMovementDetails>();
        
        private static int _movementHandlingTickMs = 20;
        private static readonly object _movementDetailsLock = new object();
        private static readonly object _sendingFlagLock = new object();

        public GameStateHandler(ILogger logger, IGeoDataInfo geoDataInfo, ICharacterInfo characterInfo, PlayerHandler playerHandler)
        {
            _logger = logger ?? throw new Exception("Game state handler - logger cannot be NULL!");
            _geoDataInfo = geoDataInfo ?? throw new Exception("Game state handler - geo data info cannot be NULL!");
            _characterInfo = characterInfo ?? throw new Exception("Game state handler - character info cannot be NULL!");
            _playerHandler = playerHandler ?? throw new Exception("Game state handler - player handler cannot be NULL!");

            StartHandling();
        }

        private void StartHandling()
        {
            HandleCharactersMovementAsync();
        }

        public void Dispose()
        {
            _movementHandlingInProgress = false;
            
            _geoDataInfo = null;
            _characterInfo = null;

            lock (_movementDetailsLock)
            {
                foreach (CharacterMovementDetails movementDetails in _movementDetailsList)
                    movementDetails.Dispose();

                _movementDetailsList.Clear();
            }
        }

        public void SwitchCharacterMovementType(int charId)
        {
            lock (_movementDetailsLock)
            {
                _movementDetailsList.Add
                (
                    new CharacterMovementDetails
                    (
                        CharacterMovementDetails.MovementType.SwitchMovementType,
                        charId,
                        new Point3<double>(0, 0, 0),
                        new Point3<double>(0, 0, 0),
                        0,
                        new Point2<int>(0, 0),
                        new Point2<int>(0, 0)
                    )
                );
            }
        }

        public void MoveCharacterLocal(int charId, Point3<double> oldLocation, Point3<double> newLocation, int timeArrivalMs)
        {
            lock (_movementDetailsLock)
            {
                _movementDetailsList.Add
                (
                    new CharacterMovementDetails
                    (
                        CharacterMovementDetails.MovementType.Local,
                        charId,
                        oldLocation,
                        newLocation,
                        timeArrivalMs,
                        new Point2<int>(0, 0),
                        new Point2<int>(0, 0)
                    )
                );
            }
        }

        public void MoveCharacterWorld(int charId, Point2<int> oldLocation, Point2<int> newLocation)
        {
            lock (_movementDetailsLock)
            {
                _movementDetailsList.Add
                (
                    new CharacterMovementDetails
                    (
                        CharacterMovementDetails.MovementType.World,
                        charId,
                        new Point3<double>(0, 0, 0),
                        new Point3<double>(0, 0, 0),
                        0,
                        oldLocation,
                        newLocation
                    )
                );
            }
        }

        private async void SendMovementDetailsToCurrentPlayerAsync(CharacterPositionUpdateDetails positionDetails, int charId)
        {
            await Task.Factory.StartNew
            (
                () =>
                {
                    PlayerDetails player = _playerHandler.GetPlayerByCurrentCharId(charId);

                    if (player != null)
                    {
                        CommandHandler.Send(new CharPositionUpdateCmdBuilder(positionDetails), player);
                    }
                }
            );
        }

        private async void SendMovementDetailsToPlayersInLocationAsync(CharacterPositionUpdateDetails positionDetails, int wmId, bool isOnWorldMap, int parentObjectId)
        {
            this.SendingMovementDetailsInProgress = true;

            await Task.Factory.StartNew
            (
                () =>
                {
                    List<CharacterData> charactersInLocationList = _characterInfo.GetCharactersByWorldLocation(wmId, isOnWorldMap, parentObjectId);
                    PlayerDetails player = null;

                    foreach (CharacterData charData in charactersInLocationList)
                    {
                        if (charData.AccId < 0)
                            continue;

                        player = _playerHandler.GetPlayerByCurrentCharId(charData.CharId);
                        if (player == null)
                            continue;

                        CommandHandler.Send(new CharPositionUpdateCmdBuilder(positionDetails), player);
                    }
                }
            );

            this.SendingMovementDetailsInProgress = false;
        }

        private async void HandleCharactersMovementAsync()
        {
            if (_movementHandlingInProgress)
            {
                _logger.UpdateLog("Character movement handling already started!");
                return;
            }

            _movementHandlingInProgress = true;
            _logger.UpdateLog("Game state handler - character movement handling started!");

            try
            {
                List<CharacterMovementDetails> movementDetailsListTemp = new List<CharacterMovementDetails>();
                CharacterData characterData;
                List<GeoDataElement> geoDataList = new List<GeoDataElement>();
                CharacterPositionUpdateDetails positionDetailsToSend = null;
                int count = 0;
                
                int wmId;
                int parentObjectId;
                bool isOnWorldMap;

                GeoDataValidationDetails geoDataValidationDetails = null;
                bool geoMovementValid = false;
                Point3<double> lastValidMovementPoint = new Point3<double>(0, 0, 0);
                bool exitPointFound = false;
                Point3<double> exitPosition = null;
                double lastTParamValue = 0;
                int timeArrivalMsIncomplete = 0;
                bool worldMovingValid = false;
                
                do
                {
                    #region Main list item removal
                    //MAIN LIST ITEM REMOVAL
                    lock (_movementDetailsLock)
                    {
                        count = _movementDetailsList.Count;

                        if (count > 0)
                        {
                            movementDetailsListTemp = _movementDetailsList.Clone();
                            foreach (CharacterMovementDetails item in _movementDetailsList)
                                item.Dispose();

                            _movementDetailsList.Clear();
                        }
                    }

                    #endregion

                    #region Interval handling
                    //INTERVAL HANDLING
                    if (count == 0)
                    {
                        await Task.Factory.StartNew(() => Thread.Sleep(GameStateHandler._movementHandlingTickMs));
                        continue;
                    }

                    #endregion

                    #region Movement details handling
                    //MOV. DETAILS HANDLING
                    foreach (CharacterMovementDetails movementDetails in movementDetailsListTemp)
                    {
                        if (geoDataList.Count > 0)
                            geoDataList.Clear();
                        
                        if (movementDetails.CharId < 0)
                        {
                            _logger.UpdateLog("Movement details handling warning - character's ID less than 0!");
                            continue;
                        }

                        //characterData = await _characterInfo.GetCharacterByNameTaskStart(movementDetails.CharName);
                        characterData = await _characterInfo.GetCharacterByIdTaskStart(movementDetails.CharId);

                        if (characterData == null)
                        {
                            _logger.UpdateLog($"Movement details handling warning - character's ID [{movementDetails.CharId}] not found!");
                            continue;
                        }
                        
                        wmId = characterData.WmId;
                        parentObjectId = characterData.ParentObjectId;
                        isOnWorldMap = characterData.IsOnWorldMap;
                        
                        switch (movementDetails.Type)
                        {
                            case CharacterMovementDetails.MovementType.SwitchMovementType:
                                {
                                    #region Local enter/exit point veryfication
                                    //LOCAL ENTER/EXIT POINT VERYFICATION
                                    exitPointFound = false;

                                    if (characterData.IsOnWorldMap)
                                    {
                                        using (BoxedData geoBoxedData = await _geoDataInfo.GetLocalGeoDataOfExitElementsTaskStart(wmId, parentObjectId))
                                        {
                                            geoDataList = (List<GeoDataElement>)geoBoxedData.Data;
                                            if (!String.IsNullOrEmpty(geoBoxedData.Msg))
                                                _logger.UpdateLog(geoBoxedData.Msg);
                                        }

                                        exitPosition = GeoDataValidator.GetRandomExitPosition(geoDataList);

                                        if (exitPosition != null)
                                        {
                                            exitPointFound = true;

                                            //PLACING CHARACTER ON ENTRY POINT
                                            characterData.MoveCharacterLocal
                                            (
                                                exitPosition.Copy(),
                                                exitPosition.Copy(),
                                                0
                                            );
                                        }   
                                    }

                                    #endregion

                                    #region State change section
                                    //STATE CHANGE SECTION

                                    if (!exitPointFound && characterData.IsOnWorldMap)
                                    {
                                        _logger.UpdateLog($"Cannot find location exit point for character's ID [{movementDetails.CharId}] wm_id [{wmId}] parent object ID [{parentObjectId}]");
                                        SendMessageToPlayer(movementDetails.CharId, "Error: cannot find entry point!");
                                    }
                                    else
                                    {
                                        characterData.IsOnWorldMap = !isOnWorldMap;
                                        _logger.UpdateLog($"Movement type switched for character's ID [{movementDetails.CharId}] to {(characterData.IsOnWorldMap ? "world" : "local")} state");

                                        positionDetailsToSend = new CharacterPositionUpdateDetails()
                                        {
                                            MovementType = (characterData.IsOnWorldMap ? "switchmap" : "switchlocal"),
                                            CharId = movementDetails.CharId,
                                            OldLocationLocal = characterData.CurrentLoc.Copy(),
                                            NewLocationLocal = characterData.CurrentLoc.Copy(),
                                            TimeArrivalMsLocal = 0,
                                            OldLocationWorld = characterData.CurrentWorldLoc.Copy(),
                                            NewLocationWorld = characterData.CurrentWorldLoc.Copy()
                                        };

                                        if (characterData.IsOnWorldMap)
                                            SendMovementDetailsToCurrentPlayerAsync(positionDetailsToSend, movementDetails.CharId);

                                        SendMovementDetailsToPlayersInLocationAsync(positionDetailsToSend, wmId, false, parentObjectId);
                                    }

                                    #endregion
                                }
                                break;
                            case CharacterMovementDetails.MovementType.Local:
                                {
                                    #region Local movement handling
                                    //LOCAL MOVEMENT HANDLING
                                    if (isOnWorldMap)
                                    {
                                        _logger.UpdateLog($"Movement details handling warning - local movement cancelled for character's ID [{movementDetails.CharId}], reason: world map state");
                                    }
                                    else
                                    {
                                        #region Geo data validation
                                        //GEO DATA VALIDATION
                                        using
                                        (
                                            BoxedData geoBoxedData = await _geoDataInfo.GetLocalGeoDataTaskStart
                                            (
                                                wmId,
                                                parentObjectId,
                                                PointConverter.Point3DoubleToInt(movementDetails.OldLocationLocal),
                                                PointConverter.Point3DoubleToInt(movementDetails.NewLocationLocal)
                                            )
                                        )
                                        {
                                            geoDataList = (List<GeoDataElement>)geoBoxedData.Data;
                                            if (!String.IsNullOrEmpty(geoBoxedData.Msg))
                                                _logger.UpdateLog(geoBoxedData.Msg);
                                        }

                                        using (BoxedData geoValidationBoxedData = await GeoDataValidator.ValidateMovementTaskStart(geoDataList, movementDetails))
                                        {
                                            geoDataValidationDetails = (GeoDataValidationDetails)geoValidationBoxedData.Data;
                                            if (!String.IsNullOrEmpty(geoValidationBoxedData.Msg))
                                                _logger.UpdateLog(geoValidationBoxedData.Msg);

                                            geoMovementValid = geoDataValidationDetails.Valid;
                                            lastValidMovementPoint = geoDataValidationDetails.LastValidMovementPoint;
                                            lastTParamValue = geoDataValidationDetails.LastTParamValue;

                                            geoDataValidationDetails.Dispose();
                                            geoDataValidationDetails = null;
                                        }
                                        #endregion

                                        //_logger.UpdateLog($"GEO VALID [{geoMovementValid}]");
                                        //geoMovementValid = true; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                                        //CHARACTER MOVING
                                        if (geoMovementValid)
                                        {
                                            characterData.MoveCharacterLocal
                                            (
                                                movementDetails.OldLocationLocal.Copy(), 
                                                movementDetails.NewLocationLocal.Copy(), 
                                                movementDetails.TimeArrivalMsLocal
                                            );

                                            positionDetailsToSend = new CharacterPositionUpdateDetails()
                                            {
                                                MovementType = "movelocal",
                                                CharId = movementDetails.CharId,
                                                OldLocationLocal = movementDetails.OldLocationLocal.Copy(),
                                                NewLocationLocal = movementDetails.NewLocationLocal.Copy(),
                                                TimeArrivalMsLocal = movementDetails.TimeArrivalMsLocal,
                                                OldLocationWorld = new Point2<int>(0, 0),
                                                NewLocationWorld = new Point2<int>(0, 0)
                                            };

                                            SendMovementDetailsToPlayersInLocationAsync(positionDetailsToSend, wmId, isOnWorldMap, parentObjectId);
                                        }
                                        else
                                        {
                                            timeArrivalMsIncomplete = Convert.ToInt32(Convert.ToDouble(movementDetails.TimeArrivalMsLocal) * lastTParamValue);
                                            
                                            characterData.MoveCharacterLocal
                                            (
                                                movementDetails.OldLocationLocal.Copy(), 
                                                lastValidMovementPoint.Copy(),
                                                timeArrivalMsIncomplete
                                            );

                                            positionDetailsToSend = new CharacterPositionUpdateDetails()
                                            {
                                                MovementType = "movelocal",
                                                CharId = movementDetails.CharId,
                                                OldLocationLocal = movementDetails.OldLocationLocal.Copy(),
                                                NewLocationLocal = lastValidMovementPoint.Copy(),
                                                TimeArrivalMsLocal = timeArrivalMsIncomplete,
                                                OldLocationWorld = new Point2<int>(0, 0),
                                                NewLocationWorld = new Point2<int>(0, 0)
                                            };

                                            SendMovementDetailsToPlayersInLocationAsync(positionDetailsToSend, wmId, isOnWorldMap, parentObjectId);
                                        }
                                    }

                                    #endregion
                                }
                                break;
                            case CharacterMovementDetails.MovementType.World:
                                {
                                    #region World map movement handling
                                    //WORLD MAP MOVEMENT HANDLING
                                    if (!isOnWorldMap)
                                    {
                                        _logger.UpdateLog($"Movement details handling warning - world map movement cancelled for character's ID [{movementDetails.CharId}], reason: local movement state");
                                    }
                                    else
                                    {
                                        Point2<int> oldLocWorld = movementDetails.OldLocationWorld.Copy();
                                        Point2<int> newLocWorld = movementDetails.NewLocationWorld.Copy();

                                        //WORLD MOVEMENT VALIDATION
                                        worldMovingValid = true;

                                        int distanceX = Math.Abs(oldLocWorld.X - newLocWorld.X);
                                        int distanceY = Math.Abs(oldLocWorld.Y - newLocWorld.Y);

                                        if 
                                        (
                                            oldLocWorld.X != characterData.CurrentWorldLoc.X || oldLocWorld.Y != characterData.CurrentWorldLoc.Y ||
                                            distanceX > 2 || distanceY > 2
                                        )
                                        {
                                            worldMovingValid = false;
                                            //_logger.UpdateLog($"Movement details handling warning - world map movement cancelled for character's ID [{movementDetails.CharId}]");
                                        }

                                        //CHARACTER MOVING
                                        if (worldMovingValid)
                                        {
                                            WorldPlaceDataDetails worldPlaceDetails = _geoDataInfo.GetWorldPlaceDataDetails(newLocWorld.X, newLocWorld.Y);
                                            int newWmId = (worldPlaceDetails != null ? worldPlaceDetails.WmId : -1);

                                            characterData.MoveCharacterWorld(newLocWorld);
                                            
                                            positionDetailsToSend = new CharacterPositionUpdateDetails()
                                            {
                                                MovementType = "moveworld",
                                                CharId = movementDetails.CharId,
                                                OldLocationLocal = new Point3<double>(0, 0, 0),
                                                NewLocationLocal = new Point3<double>(0, 0, 0),
                                                TimeArrivalMsLocal = 0,
                                                OldLocationWorld = oldLocWorld,
                                                NewLocationWorld = newLocWorld
                                            };

                                            SendMovementDetailsToPlayersInLocationAsync(positionDetailsToSend, wmId, isOnWorldMap, parentObjectId);
                                            ChangeWmIdAsync(characterData, newWmId);
                                        }  
                                        else
                                        {
                                            SendMessageToPlayer(movementDetails.CharId, "You're moving too far");
                                        }
                                    }

                                    #endregion
                                }
                                break;
                            default:
                                throw new Exception($"unknown movement type [{movementDetails.Type.ToString()}]!");
                        }
                    }

                    #endregion

                    #region Temp. list disposal
                    //TEMP. LIST DISPOSAL
                    foreach (CharacterMovementDetails item in movementDetailsListTemp)
                        item.Dispose();

                    movementDetailsListTemp.Clear();
                    
                    #endregion
                }
                while (_movementHandlingInProgress);
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Character movement handling error: {exception.Message}");
            }
            finally
            {
                _movementHandlingInProgress = false;
                _logger.UpdateLog("Game state handler - character movement handling stopped!");
            }
        }

        private async void ChangeWmIdAsync(CharacterData characterData, int newWmId)
        {
            while (this.SendingMovementDetailsInProgress)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(50));
            }

            characterData.WmId = newWmId;
            //_logger.UpdateLog($"Character's [{characterData.Name}] wm_id set to [{characterData.WmId}]");
        }
        
        private void SendMessageToPlayer(int charId, string message)
        {
            PlayerDetails details = _playerHandler.GetPlayerByCurrentCharId(charId);
            if (details == null)
                return;

            CommandHandler.Send(new InfoCmdBuilder(message), details);
        }
    }
}
