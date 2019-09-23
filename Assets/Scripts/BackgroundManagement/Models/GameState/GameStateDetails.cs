using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using System.Collections.Generic;
using UnityEngine;

namespace BackgroundManagement.Models.GameState
{
    public class GameStateDetails
    {
        private readonly object _dataLock = new object();

        private int _charId = -1;
        public int CharId
        {
            get { lock (_dataLock) { return _charId; } }
            set { lock (_dataLock) { _charId = value; } }
        }

        private int _wmId = -1;
        public int WmId
        {
            get { lock (_dataLock) { return _wmId; } }
            set { lock (_dataLock) { _wmId = value; } }
        }

        private int _mapWidth = 0;
        public int MapWidth
        {
            get { lock (_dataLock) { return _mapWidth; } }
            set { lock (_dataLock) { _mapWidth = value; } }
        }

        private int _mapHeight = 0;
        public int MapHeight
        {
            get { lock (_dataLock) { return _mapHeight; } }
            set { lock (_dataLock) { _mapHeight = value; } }
        }

        private Vector3 _localBound = new Vector3(0f, 0f, 0f); //XYZ orientation
        public Vector3 LocalBound 
        {
            get { lock (_dataLock) { return _localBound; } }
            set { lock (_dataLock) { _localBound = value; } }
        }

        private bool _isOnWorldMap = true;
        public bool IsOnWorldMap
        {
            get { lock (_dataLock) { return _isOnWorldMap; } }
            set { lock (_dataLock) { _isOnWorldMap = value; } }
        }

        private Vector3 _position = new Vector3(0f, 0f, 0f); //XYZ orientation
        public Vector3 Position
        {
            get { lock (_dataLock) { return _position; } }
            set { lock (_dataLock) { _position = value; } }
        }

        private bool _worldPlaceDetailsListConfirmed = false;
        public bool WorldPlaceDetailsListConfirmed
        {
            get { lock (_dataLock) { return _worldPlaceDetailsListConfirmed; } }
            set { lock (_dataLock) { _worldPlaceDetailsListConfirmed = value; } }
        }

        private bool _localTerrainDetailsListConfirmed = false;
        public bool LocalTerrainDetailsListConfirmed
        {
            get { lock (_dataLock) { return _localTerrainDetailsListConfirmed; } }
            set { lock (_dataLock) { _localTerrainDetailsListConfirmed = value; } }
        }

        private bool _localCharacterDetailsListConfirmed = false;
        public bool LocalCharacterDetailsListConfirmed
        {
            get { lock (_dataLock) { return _localCharacterDetailsListConfirmed; } }
            set { lock (_dataLock) { _localCharacterDetailsListConfirmed = value; } }
        }

        private List<WorldPlaceDataDetails> _worldPlaceDetailsList = new List<WorldPlaceDataDetails>();
        private List<TerrainDetails> _localTerrainDetailsList = new List<TerrainDetails>();
        private List<LocalCharacterDetails> _localCharacterDetailsList = new List<LocalCharacterDetails>();

        public List<WorldPlaceDataDetails> GetWorldPlaceDataDetails()
        {
            List<WorldPlaceDataDetails> result = null;

            lock (_dataLock)
            {
                result = _worldPlaceDetailsList.Clone();
            }

            return result;
        }

        public List<TerrainDetails> GetLocalTerrainDetails()
        {
            List<TerrainDetails> result = null;

            lock (_dataLock)
            {
                result = _localTerrainDetailsList.Clone();
            }

            return result;
        }

        public List<LocalCharacterDetails> GetLocalCharacterDetails()
        {
            List<LocalCharacterDetails> result = null;

            lock (_dataLock)
            {
                result = _localCharacterDetailsList.Clone();
            }

            return result;
        }

        public void AddOrUpdateWorldPlaceDetails(WorldPlaceDataDetails details)
        {
            bool found = false;

            lock (_dataLock)
            {
                for (int i = 0; i < _worldPlaceDetailsList.Count; i++)
                {
                    if (_worldPlaceDetailsList[i].WmId == details.WmId)
                    {
                        _worldPlaceDetailsList[i] = details;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    _worldPlaceDetailsList.Add(details);
            }
        }

        public void AddOrUpdateLocalTerrainDetails(TerrainDetails details)
        {
            bool found = false;

            lock (_dataLock)
            {
                for (int i = 0; i < _localTerrainDetailsList.Count; i++)
                {
                    if (_localTerrainDetailsList[i].ToId == details.ToId)
                    {
                        _localTerrainDetailsList[i] = details;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    _localTerrainDetailsList.Add(details);
            }
        }

        public void AddOrUpdateLocalCharacterDetails(LocalCharacterDetails details)
        {
            bool found = false;

            lock (_dataLock)
            {
                for (int i = 0; i < _localCharacterDetailsList.Count; i++)
                {
                    if (_localCharacterDetailsList[i].CharId == details.CharId)
                    {
                        _localCharacterDetailsList[i] = details;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    _localCharacterDetailsList.Add(details);
            }
        }

        public void RemoveLocalCharacterDetails(int charId)
        {
            lock (_dataLock)
            {
                for (int i = 0; i < _localCharacterDetailsList.Count; i++)
                {
                    if (_localCharacterDetailsList[i].CharId == charId)
                    {
                        _localCharacterDetailsList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sets object's data to the basic state
        /// </summary>
        public void Reset()
        {
            //property section
            this.CharId = -1;
            this.WmId = -1;
            this.MapWidth = 0;
            this.MapHeight = 0;
            this.LocalBound = new Vector3(0f, 0f, 0f);
            this.IsOnWorldMap = true;
            this.Position = new Vector3(0f, 0f, 0f);

            this.WorldPlaceDetailsListConfirmed = false;
            this.LocalTerrainDetailsListConfirmed = false;
            this.LocalCharacterDetailsListConfirmed = false;

            //list section
            lock (_dataLock)
            {
                _worldPlaceDetailsList.Clear();
                _localTerrainDetailsList.Clear();
                _localCharacterDetailsList.Clear();
            }
        }

        public void LogCurrentState()
        {
            IChat chat = MainGameHandler.GetChatHandler();
            chat.UpdateLog("========GAME STATE DETAILS");
            chat.UpdateLog($"CharId [{this.CharId}]");
            chat.UpdateLog($"WmId [{this.WmId}]");
            chat.UpdateLog($"MapWidth [{this.MapWidth}]");
            chat.UpdateLog($"MapHeight [{this.MapHeight}]");
            chat.UpdateLog($"LocalBound [{this.LocalBound.x}, {this.LocalBound.y}, {this.LocalBound.y}]");
            chat.UpdateLog($"IsOnWorldMap [{this.IsOnWorldMap}]");
            chat.UpdateLog($"Position [{this.Position.x}, {this.Position.y}, {this.Position.z}]");
            chat.UpdateLog("========");
        }
    }
}
