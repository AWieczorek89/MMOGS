using BackgroundManagement.Interfaces;
using UnityEngine;

namespace BackgroundManagement.GameHelpers.TerrainSegmentBuilding
{
    public static class TerrainSegmentCreator
    {
        public static GameObject CreateTerrainSegment(ISpecificSceneManager sceneManager, TerrainSegmentBuilder builder)
        {
            builder.InstantiateOnScene(sceneManager);
            builder.CreateServerDataVisualization();
            builder.CreateAssetVisualizationAndColliders();
            builder.SetPosition();
            return builder.GetInstance();
        }
    }
}
