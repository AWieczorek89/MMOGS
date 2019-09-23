using BackgroundManagement.Interfaces;
using UnityEngine;

public class StartupSceneManagerHandler : MonoBehaviour, ISpecificSceneManager
{
    void Start ()
    {
        MainGameHandler.RegisterSceneManager(this);
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
