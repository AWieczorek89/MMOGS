using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using UnityEngine;

namespace BackgroundManagement.GameHelpers.LocalCharacterBuilding
{
    public abstract class LocalCharacterBuilder
    {
        protected Camera _camera = null;
        protected LocalCharacterDetails _details = null;
        protected GameStateDetails _gameStateDetails = null;

        public LocalCharacterBuilder(LocalCharacterDetails details, GameStateDetails gameStateDetails, Camera camera)
        {
            _details = details;
            _gameStateDetails = gameStateDetails;
            _camera = camera;
        }

        public abstract void InstantiateOnScene(ISpecificSceneManager sceneManager);
        public abstract void SetCameraInstance();
        public abstract void SetAppearance();
        public abstract void SetLocalCharacterHandler();
        public abstract void SetMovementAndPosition();
        public abstract GameObject GetInstance();
    }
}
