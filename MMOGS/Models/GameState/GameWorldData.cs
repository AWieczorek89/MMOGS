using MMOGS.DataHandlers;
using MMOGS.Interfaces;
using MMOGS.Measurement;
using MMOGS.Measurement.Units;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.Database;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MMOGS.Models.GameState
{
    public class GameWorldData : IDisposable, IGeoDataInfo, ICharacterInfo
    {
        private static GameWorldData _lastInstance = null;

        public bool IsLoaded { get; private set; } = false;
        public bool LoadingError { get; private set; } = false;

        public int MapWidth { get; private set; } = -1;
        public int MapHeight { get; private set; } = -1;
        public int StartingPosX { get; private set; } = -1;
        public int StartingPosY { get; private set; } = -1;
        public int LocalBoundX { get; private set; } = -1;
        public int LocalBoundY { get; private set; } = -1;
        public int LocalBoundZ { get; private set; } = -1;

        private ILogger _logger = null;
        private WorldPlaceData[,] _worldPlaces = null;
        private List<DbTerrainObjectDefinitions> _terrainObjectDefinitionList = new List<DbTerrainObjectDefinitions>();
        private CharacterDataContainer _characterContainer = null;

        private readonly object _terrainDefLock = new object();
        private readonly object _worldPlacesLock = new object();

        public static GameWorldData GetLastInstance()
        {
            if (GameWorldData._lastInstance == null)
                throw new Exception("Getting last instance of GameWorldData failed - instance is NULL!");

            return GameWorldData._lastInstance;
        }

        public GameWorldData(ILogger logger)
        {
            _logger = logger;
            GameWorldData._lastInstance = this;
        }

        ~GameWorldData()
        {
            ClearData();
        }

        public void Dispose()
        {
            ClearData();
        }

        private void ClearData()
        {
            #region World places disposal

            lock(_worldPlacesLock)
            {
                if (_worldPlaces != null)
                {
                    for (int y = 0; y < _worldPlaces.GetLength(1); y++)
                    {
                        for (int x = 0; x < _worldPlaces.GetLength(0); x++)
                        {
                            if (_worldPlaces[x, y] != null)
                            {
                                _worldPlaces[x, y].Dispose();
                                _worldPlaces[x, y] = null;
                            }
                        }
                    }
                }
            }
            
            #endregion

            if (_characterContainer != null)
                _characterContainer.Dispose();
            
            lock (_terrainDefLock)
            {
                _terrainObjectDefinitionList.Clear();
            }
        }

        public WorldPlaceDataDetails GetWorldPlaceDataDetails(int coordX, int coordY)
        {
            if (coordX >= this.MapWidth || coordY >= this.MapHeight || coordX < 0 || coordY < 0)
                return null;
            
            WorldPlaceData placeData = _worldPlaces[coordX, coordY];
            if (placeData == null)
                return null;

            return new WorldPlaceDataDetails()
            {
                WorldPosX = placeData.WorldPosX,
                WorldPosY = placeData.WorldPosY,
                WmId = placeData.WmId,
                PlaceType = (int)placeData.Type,
                PlaceName = placeData.PlaceName
            };
        }

        public List<WorldPlaceDataDetails> GetWorldPlaceDataDetails()
        {
            List<WorldPlaceDataDetails> resultList = new List<WorldPlaceDataDetails>();
            WorldPlaceData placeData;

            for (int y = 0; y < this.MapHeight; y++)
            {
                for (int x = 0; x < this.MapWidth; x++)
                {
                    placeData = _worldPlaces[x, y];
                    if (placeData == null)
                        continue;

                    WorldPlaceDataDetails result = new WorldPlaceDataDetails()
                    {
                        WorldPosX = placeData.WorldPosX,
                        WorldPosY = placeData.WorldPosY,
                        WmId = placeData.WmId,
                        PlaceType = (int)placeData.Type,
                        PlaceName = placeData.PlaceName
                    };

                    resultList.Add(result);
                }
            }

            return resultList;
        }

        public async void LoadWorldDataAsync(MySqlDbManager dbManager)
        {
            this.IsLoaded = false;
            this.LoadingError = false;
            _logger.UpdateLog("Loading game world data...");

            try
            {
                if (dbManager.GetConnectionState() != System.Data.ConnectionState.Open)
                    throw new Exception("connection not open!");

                #region World map settings
                //WORLD MAP SETTINGS
                _logger.UpdateLog("Getting world map settings...");
                using (BoxedData wmsBoxedData = await dbManager.GetWorldMapSettingsDataTaskStart())
                {
                    List<DbWorldMapSettingsData> wmsDataList = (List<DbWorldMapSettingsData>)wmsBoxedData.Data;
                    if (!String.IsNullOrEmpty(wmsBoxedData.Msg))
                        _logger.UpdateLog(wmsBoxedData.Msg);

                    ExtractWorldMapSettingsData(wmsDataList);
                }

                #endregion

                #region Terrain object definitions
                //TERRAIN OBJECT DEFINITIONS

                lock (_terrainDefLock)
                {
                    _terrainObjectDefinitionList.Clear();
                }
                
                _logger.UpdateLog("Getting terrain object definitions...");

                using (BoxedData todBoxedData = await dbManager.GetTerrainObjectDefinitionsDataTaskStart())
                {
                    lock (_terrainDefLock)
                    {
                        _terrainObjectDefinitionList = (List<DbTerrainObjectDefinitions>)todBoxedData.Data;
                    }
                    
                    if (!String.IsNullOrEmpty(todBoxedData.Msg))
                        _logger.UpdateLog(todBoxedData.Msg);
                }

                #endregion

                #region World place data
                //WORLD PLACE DATA INITIALIZATION
                if (this.MapWidth <= 0 || this.MapHeight <= 0)
                    throw new Exception($"wrong map dimensions [{this.MapWidth}, {this.MapHeight}]!");

                _logger.UpdateLog("World place data initialization...");
                _worldPlaces = new WorldPlaceData[this.MapWidth, this.MapHeight];
                
                await Task.Factory.StartNew
                (
                    () =>
                    {
                        for (int y = 0; y < this.MapHeight; y++)
                        {
                            for (int x = 0; x < this.MapWidth; x++)
                            {
                                _worldPlaces[x, y] = new WorldPlaceData(_logger, this, _terrainObjectDefinitionList, x, y);
                            }
                        }
                    }
                );
                
                //WORLD PLACE DATA FILLING
                _logger.UpdateLog("Filling world place data...");

                using (BoxedData wmBoxedData = await dbManager.GetWorldMapDataTaskStart())
                {
                    List<DbWorldMapData> wmDataList = (List<DbWorldMapData>)wmBoxedData.Data;
                    if (!String.IsNullOrEmpty(wmBoxedData.Msg))
                        _logger.UpdateLog(wmBoxedData.Msg);

                    await Task.Factory.StartNew(() => FillWorldPlaceData(wmDataList));
                }

                #endregion

                #region Place instance creation
                //PLACE INSTANCE CREATION

                {
                    int wmId = -1;
                    List<DbTerrainObjectsData> terrainObjectsList = new List<DbTerrainObjectsData>();
                    List<int> parentIdList = new List<int>();

                    for (int y = 0; y < this.MapHeight; y++)
                    {
                        for (int x = 0; x < this.MapWidth; x++)
                        {
                            parentIdList.Clear();
                            terrainObjectsList.Clear();

                            wmId = _worldPlaces[x, y].WmId;

                            using (BoxedData parentIdsData = await dbManager.GetParentObjectIdsTaskStart(wmId))
                            {
                                parentIdList = (List<int>)parentIdsData.Data;
                                if (!String.IsNullOrEmpty(parentIdsData.Msg))
                                    _logger.UpdateLog(parentIdsData.Msg);

                                LogParentIds(x, y, parentIdList);
                            }
                            
                            foreach (int parentId in parentIdList)
                            {
                                using (BoxedData terrainObjectsData = await dbManager.GetTerrainObjectsDataTaskStart($"`to_wm_id` = {wmId} AND `to_parent_id` = {parentId}"))
                                {
                                    terrainObjectsList = (List<DbTerrainObjectsData>)terrainObjectsData.Data;
                                    if (!String.IsNullOrEmpty(terrainObjectsData.Msg))
                                        _logger.UpdateLog(terrainObjectsData.Msg);

                                    _worldPlaces[x, y].AddPredefinedInstanceAsync(terrainObjectsList, parentId);

                                    while (WorldPlaceData.InstanceCreationInProgress)
                                        await Task.Factory.StartNew(() => Thread.Sleep(50));
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Characters
                //CHARACTER DATA INITIALIZATION
                _logger.UpdateLog("Getting characters data...");
                _characterContainer = new CharacterDataContainer();
                bool charInitSuccess = false;

                using (BoxedData dbCharsData = await dbManager.GetCharactersDataTaskStart())
                {
                    List<DbCharactersData> charDataList = (List<DbCharactersData>)dbCharsData.Data;
                    if (!String.IsNullOrEmpty(dbCharsData.Msg)) _logger.UpdateLog(dbCharsData.Msg);

                    using (BoxedData charContainerInitData = await _characterContainer.InitializeCharacterDataTaskStart(charDataList))
                    {
                        charInitSuccess = (bool)charContainerInitData.Data;
                        _logger.UpdateLog(charContainerInitData.Msg);
                    }
                }

                if (!charInitSuccess) throw new Exception("character initialization failed!");

                #endregion

                this.IsLoaded = true;
            }
            catch (Exception exception)
            {
                this.LoadingError = true;
                _logger.UpdateLog($"An error occured during world data loading: {exception.Message}");
            }

            _logger.UpdateLog($"Loading world data ended with {(this.IsLoaded ? "success" : "failure")}.");
        }

        private void LogParentIds(int posX, int posY, List<int> parentIds)
        {
            if (parentIds == null || parentIds.Count == 0)
                return;

            string logTxt = $"Parent IDs for [{posX}, {posY}]:";
            for (int i = 0; i < parentIds.Count; i++)
                logTxt += $"{(i > 0 ? ", " : " ")}{parentIds[i]}";

            _logger.UpdateLog(logTxt);
        }
        
        private void FillWorldPlaceData(List<DbWorldMapData> wmDataList)
        {
            int wmId = -1;
            int worldPosX = -1;
            int worldPosY = -1;
            int placeType = -1;
            bool isRandomPlace = true;
            string placeName = "";

            foreach (DbWorldMapData wmData in wmDataList)
            {
                wmId = wmData.WmId;
                worldPosX = wmData.WorldPosX;
                worldPosY = wmData.WorldPosY;
                placeType = wmData.PlaceType;
                isRandomPlace = wmData.IsRandomPlace;
                placeName = wmData.PlaceName;

                if (worldPosX < 0 || worldPosX >= this.MapWidth || worldPosY < 0 || worldPosY >= this.MapHeight)
                    continue;

                //Console.WriteLine($"Setting place info, wm_id [{wmId}] place type [{placeType}] is random [{isRandomPlace}] name [{placeName}]");
                _worldPlaces[worldPosX, worldPosY].SetPlaceInfo(wmId, placeType, isRandomPlace, placeName);
            }
        }

        private void ExtractWorldMapSettingsData(List<DbWorldMapSettingsData> wmsDataList)
        {
            foreach (DbWorldMapSettingsData wmsData in wmsDataList)
            {
                _logger.UpdateLog($"Getting world map data from data type [{wmsData.Type}]...");
                switch (wmsData.Type)
                {
                    case 1:
                        this.MapWidth = Convert.ToInt32(wmsData.Value);
                        _logger.UpdateLog($"Map width set to [{this.MapWidth}]");
                        break;
                    case 2:
                        this.MapHeight = Convert.ToInt32(wmsData.Value);
                        _logger.UpdateLog($"Map height set to [{this.MapHeight}]");
                        break;
                    case 3:
                        this.StartingPosX = Convert.ToInt32(wmsData.Value);
                        _logger.UpdateLog($"Starting position X set to [{this.StartingPosX}]");
                        break;
                    case 4:
                        this.StartingPosY = Convert.ToInt32(wmsData.Value);
                        _logger.UpdateLog($"Starting position Y set to [{this.StartingPosY}]");
                        break;
                    case 5:
                        this.LocalBoundX = Convert.ToInt32(wmsData.Value);
                        _logger.UpdateLog($"Local bound X set to [{this.LocalBoundX}]");
                        break;
                    case 6:
                        this.LocalBoundY = Convert.ToInt32(wmsData.Value);
                        _logger.UpdateLog($"Local bound Y set to [{this.LocalBoundY}]");
                        break;
                    case 7:
                        this.LocalBoundZ = Convert.ToInt32(wmsData.Value);
                        _logger.UpdateLog($"Local bound Z set to [{this.LocalBoundZ}]");
                        break;
                    default:
                        throw new Exception($"cannot extract world map data from data type [{wmsData.Type}]!");
                }
            }
        }

        public Task<BoxedData> GetLocalGeoDataOfExitElementsTaskStart(int wmId, int parentObjectId)
        {
            var t = new Task<BoxedData>(() => GetLocalGeoDataOfExitElements(wmId, parentObjectId));
            t.Start();
            return t;
        }

        public BoxedData GetLocalGeoDataOfExitElements(int wmId, int parentObjectId)
        {
            BoxedData data = new BoxedData();
            List<GeoDataElement> resultList = null;
            string msg = "";

            try
            {
                WorldPlaceData worldPlace = GetWorldPlaceByWmId(wmId);
                if (worldPlace == null)
                    throw new Exception("world place not found!");

                PlaceInstance placeInstance = worldPlace.GetPredefinedInstanceByParentObjectId(parentObjectId);
                if (placeInstance == null)
                    throw new Exception("place instance not found!");

                resultList = placeInstance.GetGeoDataOfExitElements();
            }
            catch (Exception exception)
            {
                msg = $"An error occured while getting geo data (exit elements), wm_id [{wmId}] parent obj, ID [{parentObjectId}]: {exception.Message}";
            }

            data.Data = (resultList ?? new List<GeoDataElement>());
            data.Msg = msg;
            return data;
        }

        public Task<BoxedData> GetLocalGeoDataTaskStart(int wmId, int parentObjectId, Point3<int> from, Point3<int> to)
        {
            var t = new Task<BoxedData>(() => GetLocalGeoData(wmId, parentObjectId, from, to));
            t.Start();
            return t;
        }

        public BoxedData GetLocalGeoData(int wmId, int parentObjectId, Point3<int> from, Point3<int> to)
        {
            BoxedData data = new BoxedData();
            List<GeoDataElement> resultList = null;
            string msg = "";

            try
            {
                #region Bounds
                
                int pointFromX = from.X - DbTerrainObjectDefinitions.MaxCollisionXOfAllObjects;
                int pointFromY = from.Y - DbTerrainObjectDefinitions.MaxCollisionYOfAllObjects;
                int pointFromZ = from.Z - DbTerrainObjectDefinitions.MaxCollisionZOfAllObjects;
                //Console.WriteLine($"mod. point from [{pointFromX}, {pointFromY}, {pointFromZ}]");

                Point3<int> pointFromTemp = new Point3<int> 
                (
                    (pointFromX <= to.X ? pointFromX - 1 : to.X - 1), //NOTE: -1 to prevent rounding error
                    (pointFromY <= to.Y ? pointFromY - 1 : to.Y - 1), //NOTE: -1 to prevent rounding error
                    (pointFromZ <= to.Z ? pointFromZ - 1 : to.Z - 1) //NOTE: -1 to prevent rounding error
                );

                Point3<int> pointToTemp = new Point3<int>
                (
                    (from.X >= to.X ? from.X + 1 : to.X + 1), //NOTE: +1 to prevent rounding error
                    (from.Y >= to.Y ? from.Y + 1 : to.Y + 1), //NOTE: +1 to prevent rounding error
                    (from.Z >= to.Z ? from.Z + 1 : to.Z + 1) //NOTE: +1 to prevent rounding error
                );

                #endregion

                WorldPlaceData worldPlace = GetWorldPlaceByWmId(wmId);
                if (worldPlace == null)
                    throw new Exception("world place not found!");

                PlaceInstance placeInstance = worldPlace.GetPredefinedInstanceByParentObjectId(parentObjectId);
                if (placeInstance == null)
                    throw new Exception("place instance not found!");

                resultList = placeInstance.GetGeoData(pointFromTemp, pointToTemp);
            }
            catch (Exception exception)
            {
                msg = $"An error occured while getting geo data, wm_id [{wmId}] parent obj, ID [{parentObjectId}]: {exception.Message}";
            }

            data.Data = (resultList ?? new List<GeoDataElement>());
            data.Msg = msg;
            return data;
        }

        public Point2<int> GetWorldCoordsByWmId(int wmId)
        {
            Point2<int> result = new Point2<int>(-1, -1);
            WorldPlaceData wpData = GetWorldPlaceByWmId(wmId);
            if (wpData == null)
                throw new Exception($"(getting coords from wm_id) place data of wm_id [{wmId}] not found!");

            result.X = wpData.WorldPosX;
            result.Y = wpData.WorldPosY;
            return result;
        }

        public WorldPlaceData GetWorldPlaceByWmId(int wmId)
        {
            WorldPlaceData wpData = null;
            WorldPlaceData wpDataTemp = null;
            bool found = false;

            lock (_worldPlacesLock)
            {
                for (int y = 0; y < _worldPlaces.GetLength(1); y++)
                {
                    if (found)
                        break;

                    for (int x = 0; x < _worldPlaces.GetLength(0); x++)
                    {
                        wpDataTemp = _worldPlaces[x, y];
                        if (wpDataTemp == null)
                            continue;

                        if (wpDataTemp.WmId == wmId)
                        {
                            wpData = wpDataTemp;
                            found = true;
                            break;
                        }
                    }
                }
            }

            return wpData;
        }

        public Task<CharacterData> GetCharacterByNameTaskStart(string charName)
        {
            var t = new Task<CharacterData>(() => GetCharacterByName(charName));
            t.Start();
            return t;
        }

        public CharacterData GetCharacterByName(string charName)
        {
            return _characterContainer.GetCharacterByName(charName);
        }

        public List<CharacterData> GetCharactersById(IList<int> charIdList)
        {
            return _characterContainer.GetCharactersById(charIdList);
        }

        public Task<List<CharacterData>> GetCharactersByIdTaskStart(IList<int> charIdList)
        {
            var t = new Task<List<CharacterData>>(() => GetCharactersById(charIdList));
            t.Start();
            return t;
        }

        public List<CharacterData> GetCharactersByWorldLocation(int wmId, bool isOnWorldMap, int parentObjectId)
        {
            return _characterContainer.GetCharactersByWorldLocation(wmId, isOnWorldMap, parentObjectId);
        }

        public Task<List<CharacterData>> GetCharactersByWorldLocationTaskStart(int wmId, bool isOnWorldMap, int parentObjectId)
        {
            var t = new Task<List<CharacterData>>(() => GetCharactersByWorldLocation(wmId, isOnWorldMap, parentObjectId));
            t.Start();
            return t;
        }

        public CharacterData GetCharacterById(int charId)
        {
            return _characterContainer.GetCharacterById(charId);
        }

        public Task<CharacterData> GetCharacterByIdTaskStart(int charId)
        {
            var t = new Task<CharacterData>(() => GetCharacterById(charId));
            t.Start();
            return t;
        }

        public List<CharacterData> GetCharactersByAccId(int accId)
        {
            return _characterContainer.GetCharactersByAccId(accId);
        }
    }
}
