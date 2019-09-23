using MMOGS.Interfaces;
using MMOGS.Models.Database;
using System;
using System.Collections.Generic;

namespace MMOGS.Models.GameState
{
    public class WorldPlaceData : IDisposable
    {
        public enum PlaceType
        {
            Forest = 0,
            Urban = 1,
            Sand = 2
        }

        public static bool InstanceCreationInProgress { get; private set; } = false;

        public int WorldPosX { get; private set; } = -1;
        public int WorldPosY { get; private set; } = -1;

        public int WmId { get; private set; } = -1;
        public PlaceType Type { get; private set; } = 0;
        public bool IsRandomPlace { get; private set; } = true;
        public string PlaceName { get; private set; } = "";

        private ILogger _logger = null;
        private List<PlaceInstance> _predefinedPlacesList = new List<PlaceInstance>();
        private List<PlaceInstance> _temporaryPlacesList = new List<PlaceInstance>();
        private List<DbTerrainObjectDefinitions> _terrainObjectDefinitionList = new List<DbTerrainObjectDefinitions>();
        private GameWorldData _parent = null;

        private readonly object _instanceLock = new object();
        
        public WorldPlaceData
        (
            ILogger logger,
            GameWorldData parent, 
            List<DbTerrainObjectDefinitions> terrainObjectDefinitionList, 
            int worldPosX, 
            int worldPosY
        )
        {
            _logger = logger;
            _parent = parent;
            _terrainObjectDefinitionList = terrainObjectDefinitionList;
            this.WorldPosX = worldPosX;
            this.WorldPosY = worldPosY;
        }

        ~WorldPlaceData()
        {
            ClearData();
        }

        public void Dispose()
        {
            ClearData();
        }

        private void ClearData()
        {
            _logger = null;

            lock (_instanceLock)
            {
                foreach (PlaceInstance instance in _predefinedPlacesList)
                    instance.Dispose();

                foreach (PlaceInstance instance in _temporaryPlacesList)
                    instance.Dispose();

                _predefinedPlacesList.Clear();
                _temporaryPlacesList.Clear();
            }
            
            _terrainObjectDefinitionList = null; //NOTE: no Clear() method - reference given as an argument in constructor!
            _parent = null;
        }
        
        public void SetPlaceInfo(int wmId, int type, bool isRandomPlace, string placeName)
        {
            SetPlaceInfo(wmId, (PlaceType)type, isRandomPlace, placeName);
        }

        public void SetPlaceInfo(int wmId, PlaceType type, bool isRandomPlace, string placeName)
        {
            this.WmId = wmId;
            this.Type = type;
            this.IsRandomPlace = isRandomPlace;
            this.PlaceName = placeName;
        }
        
        public async void AddPredefinedInstanceAsync(List<DbTerrainObjectsData> terrainDataList, int parentTerrainObjectId)
        {
            WorldPlaceData.InstanceCreationInProgress = true;

            try
            {
                DeletePredefinedInstanceIfExists(parentTerrainObjectId);

                PlaceInstance instance = new PlaceInstance
                (
                    _terrainObjectDefinitionList,
                    _parent.LocalBoundX,
                    _parent.LocalBoundY,
                    _parent.LocalBoundZ,
                    this.WorldPosX,
                    this.WorldPosY,
                    parentTerrainObjectId
                );

                bool terrainFillingSuccess = false;
                using (BoxedData terrainFillingPacket = await instance.FillTerrainDetailsTaskStart(terrainDataList))
                {
                    terrainFillingSuccess = (bool)terrainFillingPacket.Data;
                    if (!String.IsNullOrEmpty(terrainFillingPacket.Msg))
                        _logger.UpdateLog(terrainFillingPacket.Msg);
                }

                if (terrainFillingSuccess)
                {
                    lock (_instanceLock)
                    {
                        _predefinedPlacesList.Add(instance);
                    }
                    
                    _logger.UpdateLog($"Added predefined instance, ID [{instance.PlaceInstanceId}] world pos. [{this.WorldPosX}, {this.WorldPosY}] parent object ID [{parentTerrainObjectId}]");
                }
                else
                {
                    _logger.UpdateLog("Predefined instance creation failed!");
                }
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"An error occured during predefined instance creation: {exception.Message}");
            }

            WorldPlaceData.InstanceCreationInProgress = false;
        }

        private void DeletePredefinedInstanceIfExists(int parentTerrainObjectId)
        {
            lock (_instanceLock)
            {
                for (int i = _predefinedPlacesList.Count - 1; i >= 0; i--)
                {
                    if (_predefinedPlacesList[i].TerrainParentId == parentTerrainObjectId)
                        _predefinedPlacesList.RemoveAt(i);
                }
            }
        }

        public PlaceInstance GetPredefinedInstanceByParentObjectId(int parentTerrainObjectId) //-1 for main instance
        {
            PlaceInstance result = null;

            lock (_instanceLock)
            {
                foreach (PlaceInstance instance in _predefinedPlacesList)
                {
                    if (instance.TerrainParentId == parentTerrainObjectId)
                    {
                        result = instance;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
