using UnityEngine;

namespace BackgroundManagement.Interfaces
{
    public interface IWorldMapSectionBuilder
    {
        void InstantiateOnScene(ISpecificSceneManager sceneManager);
        void SetCameraInstance();
        void AssignGroundMaterial();
        void CreateDecoration();
        void SetPlaceInformation();
        GameObject GetInstance();
    }
}
