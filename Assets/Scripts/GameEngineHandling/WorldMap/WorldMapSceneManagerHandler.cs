using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using BackgroundManagement.GameHelpers.WorldMapSectionBuilding;
using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMapSceneManagerHandler : MonoBehaviour, ISpecificSceneManager
{
    public enum PlaceType
    {
        Forest = 0,
        Urban = 1,
        Sand = 2
    }

    private static WorldMapSceneManagerHandler _instance = null;

    public Camera _camera;
    public GameObject _loadingScreen;
    public GameObject _playerObject;

    private IChat _chat;
    private GameStateDetails _gameStateDetails;
    private GameObject[,] _worldMapSectionObjects = null;
    private static readonly float _distanceBetweenSections = 10f;

    private static readonly float _playerPositionChangeTotalTime = 1f;
    private float _playerPositionChangeCounter = 0f;
    private bool _playerMovingTrigger = false;
    private Vector2 _playerMovingFrom = new Vector2(0f, 0f);
    private Vector2 _playerMovingTo = new Vector2(0f, 0f);

    void Start ()
    {
        MainGameHandler.RegisterSceneManager(this);
        _instance = this;
        _chat = MainGameHandler.GetChatHandler();
        _gameStateDetails = MainGameHandler.GetGameStateDetails();
        WaitForServerDataAndLoadScene();
    }
	
	void Update ()
    {
        MainGameHandler.GlobalUpdate();
        HandleMouseSelection();
        HandlePlayerMoving();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void DestroyExternally(GameObject objectInstance)
    {
        Destroy(objectInstance);
    }

    public GameObject InstantiateExternally(GameObject prefab, Transform parentTransform)
    {
        return Instantiate(prefab, parentTransform);
    }

    public void SetPlayerPosition(float x, float y)
    {
        _playerObject.transform.position = new Vector3
        (
            x * _distanceBetweenSections,
            _playerObject.transform.position.y,
            y * _distanceBetweenSections
        );
    }

    public static void MovePlayerExternally(Vector2 from, Vector2 to)
    {
        if (_instance == null)
        {
            MainGameHandler.GetChatHandler().UpdateLog($"WM scene manager handler - MovePlayerExternally() - manager instance is NULL!");
            return;
        }

        _instance.MovePlayer(from, to);
    }
    
    public void MovePlayer(Vector2 from, Vector2 to)
    {
        _playerPositionChangeCounter = 0f;
        _playerMovingFrom = from;
        _playerMovingTo = to;
        _playerMovingTrigger = true;
    }

    private void HandlePlayerMoving()
    {
        if (!_playerMovingTrigger)
            return;

        _playerPositionChangeCounter += Time.deltaTime;
        
        if (_playerPositionChangeCounter >= _playerPositionChangeTotalTime)
        {
            _playerMovingTrigger = false;
            SetPlayerPosition(_playerMovingTo.x, _playerMovingTo.y);
        }
        else
        {
            float tParam = _playerPositionChangeCounter / _playerPositionChangeTotalTime;
            float newPosX = Mathf.Lerp(_playerMovingFrom.x, _playerMovingTo.x, tParam);
            float newPosY = Mathf.Lerp(_playerMovingFrom.y, _playerMovingTo.y, tParam);
            SetPlayerPosition(newPosX, newPosY);
        }
    }

    private void HandleMouseSelection()
    {
        if (_playerMovingTrigger)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            
            if (hit)
            {
                GameObject parentObject = hitInfo.transform.parent.gameObject;
                WorldMapSectionHandler wmSectionHandler = parentObject.GetComponent<WorldMapSectionHandler>();

                if (wmSectionHandler != null)
                {
                    int posX = wmSectionHandler.PositionX;
                    int posY = wmSectionHandler.PositionY;

                    CommandHandler.Send(new MoveCharRequestCmdBuilder(posX, posY));
                }
            }
        }
    }

    private async void WaitForServerDataAndLoadScene()
    {
        do
        {
            await Task.Factory.StartNew(() => Thread.Sleep(1000));
        }
        while (!_gameStateDetails.WorldPlaceDetailsListConfirmed);
        
        ReloadScene();
    }
    
    private void ReloadScene()
    {
        try
        {
            DestroyCurrentWorldMapSectionObjects();
            SetPlayerPosition(_gameStateDetails.Position.x, _gameStateDetails.Position.y);
            
            int mapWidth = _gameStateDetails.MapWidth;
            int mapHeight = _gameStateDetails.MapHeight;
            List<WorldPlaceDataDetails> worldPlaceDetailsList = _gameStateDetails.GetWorldPlaceDataDetails();

            _worldMapSectionObjects = new GameObject[mapWidth, mapHeight];
            GameObject createdMapSection = null;

            int worldPosX;
            int worldPosY;

            foreach (WorldPlaceDataDetails details in worldPlaceDetailsList)
            {
                //BUILDING SECTIONS BASED ON SERVER INSTANCES
                worldPosX = details.WorldPosX;
                worldPosY = details.WorldPosY;
                
                if (worldPosX >= mapWidth || worldPosY >= mapHeight)
                {
                    _chat.UpdateLog($"World map - ReloadScene() - WARNING - position over the constraints! Pos [{worldPosX}, {worldPosY}] constr [{mapWidth} - 1, {mapHeight} - 1]");
                    continue;
                }
                
                switch ((PlaceType)details.PlaceType)
                {
                    case PlaceType.Urban:
                        createdMapSection = WorldMapSectionCreator.CreateWorldMapSection(new UrbanWmSectionBuilder(details, _camera), this);
                        break;
                    case PlaceType.Sand:
                        createdMapSection = WorldMapSectionCreator.CreateWorldMapSection(new SandWmSectionBuilder(details, _camera), this);
                        break;
                    default: //forest
                        createdMapSection = WorldMapSectionCreator.CreateWorldMapSection(new ForestWmSectionBuilder(details, _camera), this);
                        break;
                }

                _worldMapSectionObjects[worldPosX, worldPosY] = createdMapSection;
            }

            WorldPlaceDataDetails dummyDetails;
            Transform sectionTransform;

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    //EMPTY SECTIONS FILLING
                    if (_worldMapSectionObjects[x, y] == null)
                    {
                        dummyDetails = new WorldPlaceDataDetails()
                        {
                            WorldPosX = x,
                            WorldPosY = y,
                            WmId = -1,
                            PlaceType = (int)PlaceType.Forest,
                            PlaceName = ""
                        };

                        createdMapSection = WorldMapSectionCreator.CreateWorldMapSection(new ForestWmSectionBuilder(dummyDetails, _camera), this);
                        _worldMapSectionObjects[x, y] = createdMapSection;
                    }

                    //POSITIONING
                    sectionTransform = _worldMapSectionObjects[x, y].GetComponent<Transform>();
                    sectionTransform.position = new Vector3
                    (
                        x * _distanceBetweenSections,
                        sectionTransform.position.y,
                        y * _distanceBetweenSections
                    );
                }
            }

            HideLoadingScreen();
        }
        catch (Exception exception)
        {
            MainGameHandler.ShowMessageBox($"Failed to reload world map scene: {exception.Message}");
        }
    }

    private void DestroyCurrentWorldMapSectionObjects()
    {
        if (_worldMapSectionObjects == null)
            return;

        GameObject position;

        for (int x = 0; x < _worldMapSectionObjects.GetLength(0); x++)
        {
            for (int y = 0; y < _worldMapSectionObjects.GetLength(1); y++)
            {
                position = _worldMapSectionObjects[x, y];
                if (position == null)
                    continue;

                Destroy(position);
                _worldMapSectionObjects[x, y] = null;
            }
        }

        _worldMapSectionObjects = null;
    }

    private void HideLoadingScreen()
    {
        LoadingPanelHandler handler = _loadingScreen.GetComponent<LoadingPanelHandler>();
        handler.Fade();
    }
}
