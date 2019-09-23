using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using System.Collections.Generic;
using UnityEngine;

namespace BackgroundManagement.GameHelpers.TerrainSegmentBuilding
{
    public class StandardTerrainSegmentBuilder : TerrainSegmentBuilder
    {
        private static Dictionary<string, string> _terrainSegmentElementKeyCodeAndPrefabDictionary = new Dictionary<string, string>()
        {
            { "BASIC_TERRAIN" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicTerrainSegmentElement" },
            { "BASIC_WALL" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicWallSegmentElement" },
            { "BASIC_PLATFORM" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicPlatformSegmentElement" },

            { "BASIC_SLOPE_N" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicSlopeNSegmentElement" },
            { "BASIC_SLOPE_S" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicSlopeSSegmentElement" },
            { "BASIC_SLOPE_W" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicSlopeWSegmentElement" },
            { "BASIC_SLOPE_E" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicSlopeESegmentElement" },

            { "BASIC_SLOPE_CORNER_NW" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicSlopeCornerNWSegmentElement" },
            { "BASIC_SLOPE_CORNER_NE" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicSlopeCornerNESegmentElement" },
            { "BASIC_SLOPE_CORNER_SW" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicSlopeCornerSWSegmentElement" },
            { "BASIC_SLOPE_CORNER_SE" , "Prefabs/LocalPlace/TerrainSegmentElement/BasicSlopeCornerSESegmentElement" }
        };
        
        private GameObject _terrainSegmentInstance;
        private TerrainSegmentHandler _terrainSegmentHandler;
        private ISpecificSceneManager _sceneManager;

        public StandardTerrainSegmentBuilder(TerrainDetails details)
            : base(details)
        {
        }

        public override void InstantiateOnScene(ISpecificSceneManager sceneManager)
        {
            _sceneManager = sceneManager;
            GameObject terrainSegmentPrefab = (GameObject)Resources.Load("Prefabs/LocalPlace/TerrainSegment", typeof(GameObject));
            _terrainSegmentInstance = _sceneManager.InstantiateExternally(terrainSegmentPrefab, null);
            _terrainSegmentHandler = _terrainSegmentInstance.GetComponent<TerrainSegmentHandler>();
            _terrainSegmentHandler.SetTerrainDetails((TerrainDetails)_details.Clone());
        }

        public override void CreateServerDataVisualization()
        {
            _terrainSegmentHandler.AssignServerCollisionCube();
            _terrainSegmentHandler.ActivateServerColliderBox(false);
        }

        public override void CreateAssetVisualizationAndColliders()
        {
            string prefabSrc = "";

            if (_terrainSegmentElementKeyCodeAndPrefabDictionary.TryGetValue(_details.TodCode, out prefabSrc))
            {
                GameObject elementPrefab = (GameObject)Resources.Load(prefabSrc, typeof(GameObject));
                _sceneManager.InstantiateExternally(elementPrefab, _terrainSegmentInstance.transform);
            }
            else
            {
                UpdateLog($"standard terrain segment builder - cannot find terrain segment element by code [{_details.TodCode}]!");
            }
        }

        public override void SetPosition()
        {
            _terrainSegmentInstance.transform.position = new Vector3
            (
                _details.LocalPos.X,
                _details.LocalPos.Z,
                _details.LocalPos.Y
            );
        }

        public override GameObject GetInstance()
        {
            return _terrainSegmentInstance;
        }

        private void UpdateLog(string text)
        {
            IChat chat = MainGameHandler.GetChatHandler();
            chat.UpdateLog(text);
        }
    }
}
