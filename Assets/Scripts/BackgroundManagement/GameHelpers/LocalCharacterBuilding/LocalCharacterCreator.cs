using BackgroundManagement.Interfaces;
using UnityEngine;

namespace BackgroundManagement.GameHelpers.LocalCharacterBuilding
{
    public static class LocalCharacterCreator
    {
        public static GameObject CreateLocalCharacter(ISpecificSceneManager sceneManager, LocalCharacterBuilder builder)
        {
            builder.InstantiateOnScene(sceneManager);
            builder.SetCameraInstance();
            builder.SetAppearance();
            builder.SetLocalCharacterHandler();
            builder.SetMovementAndPosition();
            return builder.GetInstance();
        }
    }
}
