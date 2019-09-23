using UnityEngine;

namespace BackgroundManagement.Interfaces
{
    public interface ISpecificSceneManager
    {
        GameObject InstantiateExternally(GameObject prefab, Transform parentTransform);
        void DestroyExternally(GameObject objectInstance);
    }
}
