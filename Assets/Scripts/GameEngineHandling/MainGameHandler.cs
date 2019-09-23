using BackgroundManagement.DataHandlers;
using BackgroundManagement.DataHandlers.CommandHandling;
using BackgroundManagement.DataHandlers.LocalCommandHandling;
using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.GameState;
using MMOC.BackgroundManagement;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MainGameHandler
{
    public enum SceneType
    {
        Startup,
        Login,
        CharLobby,
        WorldMap,
        LocalPlace
    }

    private static Dictionary<SceneType, string> _sceneNameDictionary = new Dictionary<SceneType, string>()
    {
        { SceneType.Startup, "StartupScene" },
        { SceneType.Login, "LoginScene" },
        { SceneType.CharLobby, "CharLobbyScene" },
        { SceneType.WorldMap, "WorldMapScene" },
        { SceneType.LocalPlace, "LocalPlaceScene" }
    };
    
    private static readonly object _dataLock = new object();
    private static readonly string _canvasObjectName = "Canvas";
    private static readonly string _loadingPanelName = "LoadingPanel";

    #region Basic non - locked variables

    private static bool _isFirstSceneLoaded = false;
    private static Scene _currentScene;
    private static ISpecificSceneManager _sceneManagerInstance = null;
    private static bool _applicationCloseCalled = false;

    #endregion

    #region Thread - locked properties

    public static bool ApplicationCloseCalled
    {
        get { lock (_dataLock) { return _applicationCloseCalled; } }
        private set { lock (_dataLock) { _applicationCloseCalled = value; } }
    }

    public static ISpecificSceneManager SceneManagerInstance
    {
        get { lock (_dataLock) { return _sceneManagerInstance; } }
        private set { lock (_dataLock) { _sceneManagerInstance = value; } }
    }

    public static Scene CurrentScene
    {
        get { lock (_dataLock) { return _currentScene; } }
        private set { lock (_dataLock) { _currentScene = value; } }
    }

    public static bool IsFirstSceneLoaded
    {
        get { lock (_dataLock) { return _isFirstSceneLoaded; } }
        private set { lock (_dataLock) { _isFirstSceneLoaded = value; } }
    }

    #endregion

    //data objects
    private static GameStateDetails _gameStateDetails;

    //data handlers
    private static ChatHandler _chatHandler;
    private static LocalSettingsHandler _localSettingsHandler;
    private static TcpConnectionHandler _tcpConnectionHandler;
    private static CommandHandler _commandHandler;
    private static LocalCommandHandler _localCommandHandler;
    private static ConnectionChecker _connectionChecker;
    private static LobbyCharactersHandler _lobbyCharsHandler;

    [RuntimeInitializeOnLoadMethod]
    private static void OnRuntimeLoad()
    {
        InitializeData();
        InitializeHandlers();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterFirstSceneLoad()
    {
        MainGameHandler.CurrentScene = SceneManager.GetActiveScene();
        MainGameHandler.IsFirstSceneLoaded = true;
    }
    
    //NOTE: call this methos in Start() in specific scene manager objects
    public static void RegisterSceneManager(ISpecificSceneManager sceneManagerInstance)
    {
        MainGameHandler.SceneManagerInstance = sceneManagerInstance;
        MainGameHandler.CurrentScene = SceneManager.GetActiveScene();
    }

    //NOTE: call this method in Update() in specific scene manager objects
    public static void GlobalUpdate()
    {
        HandleKeys();
    }
    
    public static async void CloseApplication()
    {
        if (MainGameHandler.ApplicationCloseCalled)
            return;

        MainGameHandler.ApplicationCloseCalled = true;
        await Task.Factory.StartNew(() => Thread.Sleep(1000));

        Debug.Log("Closing app...");
        TcpServerClient.StopServerClient();
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private static void HandleKeys()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ShowOrHideChatPanel(true);
        }
        else
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShowOrHideChatPanel(false);
        }
    }

    public static void ShowMessageBox(string content)
    {
        ShowMessageBox(content, "", null);
    }

    public static GameObject FindCanvas()
    {
        GameObject canvas = null;
        GameObject[] gameObjects = MainGameHandler.CurrentScene.GetRootGameObjects();

        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.name.Equals(_canvasObjectName, StringComparison.InvariantCultureIgnoreCase))
            {
                canvas = gameObject;
                break;
            }
        }

        return canvas;
    }

    public static void ShowLoadingScreen()
    {
        if (MainGameHandler.SceneManagerInstance == null)
            return;

        try
        {
            GameObject canvas = FindCanvas();
            GameObject loadingScreenPrefab = (GameObject)Resources.Load($"Prefabs/UI/LoadingPanel/{_loadingPanelName}", typeof(GameObject));

            if (canvas == null || loadingScreenPrefab == null)
                throw new Exception("canvas or loading screen prefab is NULL!");

            Transform[] canvasChildren = canvas.GetComponentsInChildren<Transform>();
            bool found = false;

            foreach (Transform child in canvasChildren)
            {
                if (child.GetComponent<LoadingPanelHandler>() != null)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                MainGameHandler.SceneManagerInstance.InstantiateExternally(loadingScreenPrefab, canvas.transform);
        }
        catch (Exception exception)
        {
            Debug.Log($"ShowLoadingScreen(): {exception.Message}");
        }
    }

    public static void HideLoadingScreen()
    {
        if (MainGameHandler.SceneManagerInstance == null)
            return;

        try
        {
            GameObject canvas = FindCanvas();
            Transform loadingPanelTransform = canvas.transform.Find(_loadingPanelName);

            if (loadingPanelTransform == null || canvas == null)
                throw new Exception("cannot find loading panel transform or canvas");

            LoadingPanelHandler panelHandler = loadingPanelTransform.gameObject.GetComponent<LoadingPanelHandler>();
            if (panelHandler == null)
                throw new Exception("cannot find panel handler");

            panelHandler.Fade();
        }
        catch (Exception exception)
        {
            Debug.Log($"HideLoadingScreen(): {exception.Message}");
        }
    }

    public static void ShowMessageBox(string content, string title, Action actionAfterConfirmation = null)
    {
        if (MainGameHandler.SceneManagerInstance == null)
            return;

        try
        {
            GameObject canvas = FindCanvas();
            GameObject messageBoxPrefab = (GameObject)Resources.Load("Prefabs/UI/MessageBox/MessageBox", typeof(GameObject));
            
            if (canvas == null || messageBoxPrefab == null)
            {
                Debug.Log("ShowMessageBox() - cannot find canvas or message box prefab!");
                return;
            }

            GameObject newMsgBoxInstance = MainGameHandler.SceneManagerInstance.InstantiateExternally(messageBoxPrefab, canvas.transform);
            MessageBoxHandler handler = newMsgBoxInstance.GetComponent<MessageBoxHandler>();
            handler.Set(content, title, actionAfterConfirmation);
        }
        catch (Exception exception)
        {
            Debug.Log($"ShowMessageBox(): {exception.Message}");
        }
    }
    
    private static void ShowOrHideChatPanel(bool show)
    {
        if (MainGameHandler.SceneManagerInstance == null)
            return;

        try
        {
            GameObject canvas = FindCanvas();
            GameObject chatPanelPrefab = (GameObject)Resources.Load("Prefabs/UI/Chat/ChatPanel", typeof(GameObject));

            if (canvas == null || chatPanelPrefab == null)
            {
                Debug.Log("ShowChatPanel() - cannot find canvas or chat panel prefab!");
                return;
            }

            Transform[] canvasChildren = canvas.GetComponentsInChildren<Transform>();
            GameObject chatPanelInstance = null;
            
            foreach (Transform child in canvasChildren)
            {
                if (child.GetComponent<UiChatController>() != null)
                {
                    chatPanelInstance = child.gameObject;
                    break;
                }
            }

            if (show && chatPanelInstance == null)
            {
                MainGameHandler.SceneManagerInstance.InstantiateExternally(chatPanelPrefab, canvas.transform);
            }
            else
            if (!show && chatPanelInstance != null)
            {
                MainGameHandler.SceneManagerInstance.DestroyExternally(chatPanelInstance);
            }
        }
        catch (Exception exception)
        {
            Debug.Log($"ShowOrHideChatPanel(): {exception.Message}");
        }
    }

    private static void InitializeData()
    {
        //GAME STATE
        _gameStateDetails = new GameStateDetails();
    }

    private static void InitializeHandlers()
    {
        try
        {
            //CHAT
            _chatHandler = ChatHandler.GetInstance();

            //LOCAL SETTINGS
            _localSettingsHandler = LocalSettingsHandler.GetInstance(_chatHandler);

            //COMMAND HANDLER
            _commandHandler = new CommandHandler(_chatHandler);
            _commandHandler.RegisterCommandHandlingStrategy(new CmdPingResponseStrategy());
            _commandHandler.RegisterCommandHandlingStrategy(new CmdServerTimeStrategy());
            _commandHandler.RegisterCommandHandlingStrategy(new CmdCharPositionUpdateStrategy(_chatHandler, _gameStateDetails));
            _commandHandler.RegisterCommandHandlingStrategy(new CmdCharPositionBasicDetailsStrategy(_chatHandler, _gameStateDetails));
            _commandHandler.RegisterCommandHandlingStrategy(new CmdLocalTerrainDetailsStrategy(_chatHandler, _gameStateDetails));
            _commandHandler.RegisterCommandHandlingStrategy(new CmdLocalCharacterDetailsStrategy(_chatHandler, _gameStateDetails));
            _commandHandler.RegisterCommandHandlingStrategy(new CmdWorldPlaceDetailsStrategy(_chatHandler, _gameStateDetails));
            _commandHandler.RegisterCommandHandlingStrategy(new CmdChatMessageStrategy(_chatHandler));
            _commandHandler.RegisterCommandHandlingStrategy(new CmdInfoStrategy());
            _commandHandler.RegisterCommandHandlingStrategy(new CmdLoginSuccessStrategy());
            _commandHandler.RegisterCommandHandlingStrategy(new CmdLobbyInfoStrategy(_chatHandler));
            _commandHandler.RegisterCommandHandlingStrategy(new CmdCharChoosingSuccessStrategy());
            
            //LOCAL COMMAND HANDLER
            _localCommandHandler = new LocalCommandHandler(_chatHandler);
            _localCommandHandler.RegisterCommandHandlingStrategy(new LocalCmdQuitStrategy());
            _localCommandHandler.RegisterCommandHandlingStrategy(new LocalCmdShowColliderBoxesStrategy(_chatHandler));
            _localCommandHandler.RegisterCommandHandlingStrategy(new LocalCmdSwitchPlaceStrategy());

            //CONNECTION HANDLER
            _tcpConnectionHandler = TcpConnectionHandler.GetInstance(_chatHandler, _commandHandler);
            _tcpConnectionHandler.StartConnection(_localSettingsHandler.GetSettings().TcpConnSettings);

            //CONNECTION CHECKER
            _connectionChecker = new ConnectionChecker();

            //LOBBY CHARS HANDLER
            _lobbyCharsHandler = new LobbyCharactersHandler();
        }
        catch (Exception exception)
        {
            Debug.Log($"Handler initializing error: {exception.Message} | {exception.StackTrace}");
        }
    }

    public static GameStateDetails GetGameStateDetails()
    {
        return _gameStateDetails;
    }

    public static IChat GetChatHandler()
    {
        return _chatHandler;
    }

    public static ConnectionChecker GetConnectionChecker()
    {
        return _connectionChecker;
    }

    public static LobbyCharactersHandler GetLobbyCharactersHandler()
    {
        return _lobbyCharsHandler;
    }

    public static LocalCommandHandler GetLocalCommandHandler()
    {
        return _localCommandHandler;
    }

    public static ConnectionChecker.LoginState CheckLoginState()
    {
        ConnectionChecker.LoginState loginState = ConnectionChecker.LoginState.NotLoggedInOrLoginFailed;
        if (_connectionChecker != null)
            loginState = _connectionChecker.ClientLoginState;

        return loginState;
    }

    public static void ChangeScene(SceneType sceneType)
    {
        string sceneName = "";

        if (_sceneNameDictionary.TryGetValue(sceneType, out sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log($"Scene changing method - cannot get scene name of type [{sceneType.ToString()}]");
        }
    }

    public static bool CheckIfSceneActive(SceneType sceneType, Scene sceneToCheck)
    {
        bool active = false;
        string sceneName = "";
        
        if (_sceneNameDictionary.TryGetValue(sceneType, out sceneName))
        {
            active = (sceneToCheck.name.Equals(sceneName, GlobalData.InputDataStringComparison));
        }
        else
        {
            Debug.Log($"Scene checking method - cannot get scene name of type [{sceneType.ToString()}]");
        }
        
        return active;
    }
}
