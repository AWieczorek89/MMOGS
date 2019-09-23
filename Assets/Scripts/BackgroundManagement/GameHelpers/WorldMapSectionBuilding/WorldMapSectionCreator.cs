using BackgroundManagement.Interfaces;
using UnityEngine;

namespace BackgroundManagement.GameHelpers.WorldMapSectionBuilding
{
    public static class WorldMapSectionCreator
    {
        public static GameObject CreateWorldMapSection(IWorldMapSectionBuilder builder, ISpecificSceneManager sceneManager)
        {
            builder.InstantiateOnScene(sceneManager);
            builder.SetCameraInstance();
            builder.AssignGroundMaterial();
            builder.CreateDecoration();
            builder.SetPlaceInformation();
            return builder.GetInstance();
        }
    }
}
