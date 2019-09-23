using BackgroundManagement.DataHandlers;
using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using BackgroundManagement.Interfaces;
using UnityEngine;

public class CharLobbySceneManagerHandler : MonoBehaviour, ISpecificSceneManager
{
    public enum LobbyState
    {
        CharacterList,
        CharacterCreation
    }

    public GameObject _characterListPanel;
    public GameObject _characterCreationPanel;

    private static CharLobbySceneManagerHandler _lastInstance = null;

    public static CharLobbySceneManagerHandler GetLastInstance()
    {
        return _lastInstance;
    }

    private void Start()
    {
        MainGameHandler.RegisterSceneManager(this);
        _lastInstance = this;
        LoadCharacterList();
    }

    private void Update()
    {
        MainGameHandler.GlobalUpdate();
    }

    private void OnDestroy()
    {
        _lastInstance = null;
    }
    
    public void DestroyExternally(GameObject objectInstance)
    {
        Destroy(objectInstance);
    }

    public GameObject InstantiateExternally(GameObject prefab, Transform parentTransform)
    {
        return Instantiate(prefab, parentTransform);
    }

    private void LoadCharacterList()
    {
        LobbyCharactersHandler lobbyCharHandler = MainGameHandler.GetLobbyCharactersHandler();
        lobbyCharHandler.ListConfirmed = false;
        lobbyCharHandler.Clear();
        CommandHandler.Send(new GetAccountCharsCmdBuilder()); //sends request to the server
    }

    public void ChangeLobbyState(LobbyState state)
    {
        switch (state)
        {
            case LobbyState.CharacterList:
                {
                    _characterListPanel.SetActive(true);
                    _characterCreationPanel.SetActive(false);
                }
                break;
            case LobbyState.CharacterCreation:
                {
                    _characterListPanel.SetActive(false);
                    _characterCreationPanel.SetActive(true);
                }
                break;
        }
    }
}
