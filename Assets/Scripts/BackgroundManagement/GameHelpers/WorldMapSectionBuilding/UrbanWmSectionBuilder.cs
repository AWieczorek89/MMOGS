using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using UnityEngine;

namespace BackgroundManagement.GameHelpers.WorldMapSectionBuilding
{
    public class UrbanWmSectionBuilder : IWorldMapSectionBuilder
    {
        private ISpecificSceneManager _sceneManager;
        private WorldPlaceDataDetails _details;
        private Camera _camera;
        private WorldMapSectionHandler _sectionHandler;
        private GameObject _worldMapSectionInstance = null;

        public UrbanWmSectionBuilder(WorldPlaceDataDetails details, Camera camera)
        {
            _details = details;
            _camera = camera;
        }

        public void InstantiateOnScene(ISpecificSceneManager sceneManager)
        {
            _sceneManager = sceneManager;
            GameObject mapSectionPrefab = (GameObject)Resources.Load("Prefabs/WorldMap/WorldMapSection", typeof(GameObject));
            _worldMapSectionInstance = _sceneManager.InstantiateExternally(mapSectionPrefab, null);
            _sectionHandler = _worldMapSectionInstance.GetComponent<WorldMapSectionHandler>();
        }
        
        public void SetCameraInstance()
        {
            _sectionHandler.CameraInstance = _camera;
        }

        public void AssignGroundMaterial()
        {
            Material groundMaterial = (Material)Resources.Load("Materials/WorldMap/GroundMaterials/GroundGrassMaterial", typeof(Material));
            GameObject groundPlane = _worldMapSectionInstance.transform.Find("SectionPlane").gameObject;
            groundPlane.GetComponent<Renderer>().material = groundMaterial;
        }

        public void CreateDecoration()
        {
            GameObject decoPrefab = (GameObject)Resources.Load("Prefabs/WorldMap/WorldMapSectionDeco/UrbanDecoration", typeof(GameObject));
            _sceneManager.InstantiateExternally(decoPrefab, _worldMapSectionInstance.transform);
        }

        public void SetPlaceInformation()
        {
            _sectionHandler.SetPlaceInformation(_details.WorldPosX, _details.WorldPosY, _details.WmId, _details.PlaceName);
        }

        public GameObject GetInstance()
        {
            return _worldMapSectionInstance;
        }
    }
}
