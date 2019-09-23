using BackgroundManagement.DataHandlers;
using BackgroundManagement.Interfaces;
using UnityEngine;

public class LoginSceneManagerHandler : MonoBehaviour, ISpecificSceneManager
{
    void Start ()
    {
        MainGameHandler.RegisterSceneManager(this);
        ConnectionChecker.Unlock(true);
	}
	
	void Update ()
    {
        MainGameHandler.GlobalUpdate();
	}

    public GameObject InstantiateExternally(GameObject prefab, Transform parentTransform)
    {
        return Instantiate(prefab, parentTransform);
    }

    public void DestroyExternally(GameObject objectInstance)
    {
        Destroy(objectInstance);
    }
}
