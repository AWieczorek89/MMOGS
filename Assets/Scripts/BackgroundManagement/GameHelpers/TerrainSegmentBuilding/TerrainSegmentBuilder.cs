using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using UnityEngine;

namespace BackgroundManagement.GameHelpers.TerrainSegmentBuilding
{
    public abstract class TerrainSegmentBuilder
    {
        protected TerrainDetails _details = null;

        public TerrainSegmentBuilder(TerrainDetails details)
        {
            //Debug.Log($"ter. details: to_id [{details.ToId}] tod_id [{details.TodId}] t [{details.TodIsTerrain}] p [{details.TodIsPlatform}] ob [{details.TodIsObstacle}]");
            _details = details;
        }

        public abstract void InstantiateOnScene(ISpecificSceneManager sceneManager);
        public abstract void CreateServerDataVisualization();
        public abstract void CreateAssetVisualizationAndColliders();
        public abstract void SetPosition();
        public abstract GameObject GetInstance();
    }
}
