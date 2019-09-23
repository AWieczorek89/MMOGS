using MMOGS.Interfaces;
using MMOGS.Measurement;
using MMOGS.Measurement.Units;
using MMOGS.Models.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMOGS.Models.GameState
{
    public class PlaceInstance : IDisposable
    {
        private static int _placeInstanceIdCounter = -1;

        public int PlaceInstanceId { get; private set; } = -1;
        public int WorldPosX { get; private set; } = -1;
        public int WorldPosY { get; private set; } = -1;
        public int TerrainParentId { get; private set; } = -1;
        
        private PlaceInstanceTerrainDetails[,,] _terrainDetails = null;
        private List<Tuple<int, int, int>> _exitElementsIndexList = new List<Tuple<int, int, int>>();

        private List<DbTerrainObjectDefinitions> _terrainObjectDefinitionList = null;

        private readonly object _terrainDetailsLock = new object();

        /// <summary>
        /// A game place instance, containing details about terrain and geodata
        /// </summary>
        /// <param name="terrainObjectDefinitionList">Terrain definition list reference</param>
        /// <param name="boundX">Maximum position (X) of all terrain objects</param>
        /// <param name="boundY">Maximum position (Y) of all terrain objects</param>
        /// <param name="boundZ">Maximum position (Z) of all terrain objects</param>
        /// <param name="worldPosX">Position of place instance (X) on world map</param>
        /// <param name="worldPosY">Position of place instance (Y) on world map</param>
        /// <param name="terrainParentId">-1 for basic instance. ID (to_id) greater than -1 defines sub-instance, which has a parent terrain object (used for teleport) in basic instance</param>
        public PlaceInstance
        (
            List<DbTerrainObjectDefinitions> terrainObjectDefinitionList,
            int boundX, 
            int boundY, 
            int boundZ,
            int worldPosX,
            int worldPosY,
            int terrainParentId
        )
        {
            if (boundX < 1 || boundY < 1 || boundZ < 1)
                throw new Exception($"(place instance) wrong terrain bounds [{boundX}, {boundY}, {boundZ}]");

            this.PlaceInstanceId = ++PlaceInstance._placeInstanceIdCounter;
            this.WorldPosX = worldPosX;
            this.WorldPosY = worldPosY;
            this.TerrainParentId = terrainParentId;
            
            _terrainObjectDefinitionList = terrainObjectDefinitionList;
            _terrainDetails = new PlaceInstanceTerrainDetails[boundX, boundY, boundZ];
        }

        ~PlaceInstance()
        {
            ClearData();
        }

        public void Dispose()
        {
            ClearData();
        }

        public PlaceInstanceTerrainDetails[,,] GetTerrainDetails()
        {
            PlaceInstanceTerrainDetails[,,] result; 

            lock (_terrainDetailsLock)
            {
                result = new PlaceInstanceTerrainDetails[_terrainDetails.GetLength(0), _terrainDetails.GetLength(1), _terrainDetails.GetLength(2)];

                for (int z = 0; z < _terrainDetails.GetLength(2); z++)
                {
                    for (int y = 0; y < _terrainDetails.GetLength(1); y++)
                    {
                        for (int x = 0; x < _terrainDetails.GetLength(0); x++)
                        {
                            if (_terrainDetails[x, y, z] != null)
                                result[x, y, z] = _terrainDetails[x, y, z].Copy();
                            else
                                result[x, y, z] = null;
                        }
                    }
                }
            }

            return result;
        }

        private void ClearData()
        {
            #region Terrain details disposal
            lock (_terrainDetailsLock)
            {
                if (_terrainDetails != null)
                {
                    for (int z = 0; z < _terrainDetails.GetLength(2); z++)
                    {
                        for (int y = 0; y < _terrainDetails.GetLength(1); y++)
                        {
                            for (int x = 0; x < _terrainDetails.GetLength(0); x++)
                            {
                                if (_terrainDetails[x, y, z] != null)
                                {
                                    _terrainDetails[x, y, z].Dispose();
                                    _terrainDetails[x, y, z] = null;
                                }
                            }
                        }
                    }
                }

                _exitElementsIndexList.Clear();
            }
            #endregion
            
            _terrainObjectDefinitionList = null; //NOTE: no Clear() method - reference given as an argument in constructor!
        }

        public List<GeoDataElement> GetGeoDataOfExitElements()
        {
            List<GeoDataElement> geoDataList = new List<GeoDataElement>();
            
            lock (_terrainDetailsLock)
            {
                PlaceInstanceTerrainDetails details;
                DbTerrainObjectDefinitions terrainDefinition;
                Point3<int> location;
                Point3<int> colliderSize;
                GeoDataElement.Type elementType = GeoDataElement.Type.None;
                bool isExit = false;

                int x, y, z;

                foreach (Tuple<int, int, int> indexes in _exitElementsIndexList)
                {
                    x = indexes.Item1;
                    y = indexes.Item2;
                    z = indexes.Item3;

                    details = _terrainDetails[x, y, z];
                    if (details == null)
                        continue;

                    terrainDefinition = details.TerrainDefinition;

                    if (!terrainDefinition.IsTerrain && !terrainDefinition.IsPlatform && !terrainDefinition.IsObstacle)
                        continue;

                    if (terrainDefinition.IsObstacle) elementType = GeoDataElement.Type.Obstacle;
                    else if (terrainDefinition.IsTerrain) elementType = GeoDataElement.Type.Terrain;
                    else if (terrainDefinition.IsPlatform) elementType = GeoDataElement.Type.Platform;
                    else elementType = GeoDataElement.Type.None;

                    location = new Point3<int>(details.LocalPosX, details.LocalPosY, details.LocalPosZ);
                    colliderSize = new Point3<int>(terrainDefinition.CollisionX, terrainDefinition.CollisionY, terrainDefinition.CollisionZ);
                    isExit = details.IsExit;

                    GeoDataElement geoDataElement = new GeoDataElement(location, colliderSize, elementType, isExit);
                    geoDataList.Add(geoDataElement);
                }
            }

            return geoDataList;
        }

        public List<GeoDataElement> GetGeoData(Point3<int> pointFrom, Point3<int> pointTo)
        {
            List<GeoDataElement> geoDataList = new List<GeoDataElement>();
            
            lock (_terrainDetailsLock)
            {
                pointFrom.X = Measure.Clamp(0, _terrainDetails.GetLength(0) - 1, pointFrom.X);
                pointFrom.Y = Measure.Clamp(0, _terrainDetails.GetLength(1) - 1, pointFrom.Y);
                pointFrom.Z = Measure.Clamp(0, _terrainDetails.GetLength(2) - 1, pointFrom.Z);
                pointTo.X = Measure.Clamp(0, _terrainDetails.GetLength(0) - 1, pointTo.X);
                pointTo.Y = Measure.Clamp(0, _terrainDetails.GetLength(1) - 1, pointTo.Y);
                pointTo.Z = Measure.Clamp(0, _terrainDetails.GetLength(2) - 1, pointTo.Z);

                PlaceInstanceTerrainDetails details;
                DbTerrainObjectDefinitions terrainDefinition;
                Point3<int> location;
                Point3<int> colliderSize;
                GeoDataElement.Type elementType = GeoDataElement.Type.None;
                bool isExit = false;

                
                for (int z = pointFrom.Z; z <= pointTo.Z; z++)
                {
                    for (int y = pointFrom.Y; y <= pointTo.Y; y++)
                    {
                        for (int x = pointFrom.X; x <= pointTo.X; x++)
                        {
                            details = _terrainDetails[x, y, z];
                            if (details == null)
                                continue;

                            terrainDefinition = details.TerrainDefinition;

                            if (!terrainDefinition.IsTerrain && !terrainDefinition.IsPlatform && !terrainDefinition.IsObstacle)
                                continue;

                            if (terrainDefinition.IsObstacle) elementType = GeoDataElement.Type.Obstacle;
                            else if (terrainDefinition.IsTerrain) elementType = GeoDataElement.Type.Terrain;
                            else if (terrainDefinition.IsPlatform) elementType = GeoDataElement.Type.Platform;
                            else elementType = GeoDataElement.Type.None;

                            location = new Point3<int>(details.LocalPosX, details.LocalPosY, details.LocalPosZ);
                            colliderSize = new Point3<int>(terrainDefinition.CollisionX, terrainDefinition.CollisionY, terrainDefinition.CollisionZ);
                            isExit = details.IsExit;

                            GeoDataElement geoDataElement = new GeoDataElement(location, colliderSize, elementType, isExit);
                            geoDataList.Add(geoDataElement);
                        }
                    }
                }
                
            }

            return geoDataList;
        }

        private DbTerrainObjectDefinitions GetTerrainDefinition(int todId)
        {
            DbTerrainObjectDefinitions result = null;

            foreach (DbTerrainObjectDefinitions definition in _terrainObjectDefinitionList)
            {
                if (definition.TodId == todId)
                {
                    result = definition;
                    break;
                }
            }

            return result;
        }

        public Task<BoxedData> FillTerrainDetailsTaskStart(List<DbTerrainObjectsData> dataList)
        {
            var t = new Task<BoxedData>(() => FillTerrainDetails(dataList));
            t.Start();
            return t;
        }

        private BoxedData FillTerrainDetails(List<DbTerrainObjectsData> terrainDataList)
        {
            BoxedData data = new BoxedData();
            bool success = false;
            string msg = "";

            try
            {
                int toId;
                int wmId;
                int localPosX;
                int localPosY;
                int localPosZ;
                int todId;
                int parentId;
                bool isParentalTeleport;
                bool isExit;
                DbTerrainObjectDefinitions terrainDefinition;

                lock (_terrainDetailsLock)
                {
                    foreach (DbTerrainObjectsData terrainData in terrainDataList)
                    {
                        toId = terrainData.ToId;
                        wmId = terrainData.WmId;
                        localPosX = terrainData.LocalPosX;
                        localPosY = terrainData.LocalPosY;
                        localPosZ = terrainData.LocalPosZ;
                        todId = terrainData.TodId;
                        parentId = terrainData.ParentId;
                        isParentalTeleport = terrainData.IsParentalTeleport;
                        isExit = terrainData.IsExit;
                        terrainDefinition = (todId > -1 ? GetTerrainDefinition(todId) : null);

                        if
                        (
                            localPosX < 0 || localPosX >= _terrainDetails.GetLength(0) ||
                            localPosY < 0 || localPosY >= _terrainDetails.GetLength(1) ||
                            localPosZ < 0 || localPosZ >= _terrainDetails.GetLength(2)
                        )
                        { continue; }

                        //Console.WriteLine($"Terrain details - wm_id [{wmId}] local pos [{localPosX}, {localPosY}, {localPosZ}] parent [{parentId}] is TP [{isParentalTeleport}] exit [{isExit}]");
                        _terrainDetails[localPosX, localPosY, localPosZ] = new PlaceInstanceTerrainDetails
                        (
                            toId,
                            wmId,
                            localPosX,
                            localPosY,
                            localPosZ,
                            parentId,
                            isParentalTeleport,
                            isExit,
                            terrainDefinition
                        );

                        if (isExit)
                            _exitElementsIndexList.Add(Tuple.Create(localPosX, localPosY, localPosZ));
                    }
                }
                
                success = true;
            }
            catch (Exception exception)
            {
                msg = $"Error - terrain details filling - place instance ID [{this.PlaceInstanceId}]: {exception.Message}";
            }

            data.Data = success;
            data.Msg = msg;
            return data;
        }
    }
}
