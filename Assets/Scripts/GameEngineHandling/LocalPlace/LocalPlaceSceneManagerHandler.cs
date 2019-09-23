using BackgroundManagement.GameHelpers.LocalCharacterBuilding;
using BackgroundManagement.GameHelpers.TerrainSegmentBuilding;
using BackgroundManagement.Interfaces;
using BackgroundManagement.Measurement;
using BackgroundManagement.Measurement.Units;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocalPlaceSceneManagerHandler : MonoBehaviour, ISpecificSceneManager
{
    public GameObject _loadingScreen;
    public Camera _mainCamera;
    public GameObject _tppCameraManager;
    public GameObject _groundSelectionMarkPrefab;
    public GameObject _playerPositionMark;
    public GameObject _boundingBoxObject;
    public float _camRayDistance = 1000f;

    private static LocalPlaceSceneManagerHandler _instance = null;
    private IChat _chat;
    private GameStateDetails _gameStateDetails;
    
    private List<GameObject> _terrainSegmentObjectList = new List<GameObject>();
    private List<GameObject> _localCharacterObjectList = new List<GameObject>();
    private GameObject _playerCharacter = null;

    private void Start ()
    {
        _instance = this;
        MainGameHandler.RegisterSceneManager(this);
        _chat = MainGameHandler.GetChatHandler();
        _gameStateDetails = MainGameHandler.GetGameStateDetails();
        WaitForServerDataAndLoadScene();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void Update ()
    {
        MainGameHandler.GlobalUpdate();
        HandleGroundSelection();
    }

    public static void AddNewNonPlayerCharacterExternally(LocalCharacterDetails charDetails)
    {
        if (_instance == null || charDetails.CharId == _instance._gameStateDetails.CharId)
            return;
        
        _instance.AddNewNonPlayerCharacter(charDetails);
    }

    public static void RemoveNonPlayerCharacterExternally(int charId)
    {
        if (_instance == null || charId == _instance._gameStateDetails.CharId)
            return;

        RemoveCharacter(charId);
    }

    private static void RemoveCharacter(int charId)
    {
        _instance._gameStateDetails.RemoveLocalCharacterDetails(charId);
        
        LocalCharacterHandler charHandler;
        LocalCharacterDetails details;
        
        try
        {
            GameObject localCharObject;
            
            for (int i = 0; i < _instance._localCharacterObjectList.Count; i++)
            {
                localCharObject = _instance._localCharacterObjectList[i];
                charHandler = localCharObject.GetComponent<LocalCharacterHandler>();
                details = charHandler.GetDetails();

                if (details.CharId == charId)
                {
                    Destroy(localCharObject);
                    _instance._localCharacterObjectList.RemoveAt(i);
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            MainGameHandler.GetChatHandler().UpdateLog($"Local place manager - RemoveCharacter() - error: {exception.Message}");
        } 
    }

    public static void MovePlayerExternally(int charId, Point3<double> pointTo, int timeArrivalMs)
    {
        if (_instance == null)
            return;
        
        Vector3 newLocation = new Vector3
        (
            Convert.ToSingle(pointTo.X),
            Convert.ToSingle(pointTo.Z),
            Convert.ToSingle(pointTo.Y)
        );

        if (charId == _instance._gameStateDetails.CharId)
        {
            PlayerPositionMarkHandler markHandler = _instance._playerPositionMark.GetComponent<PlayerPositionMarkHandler>();
            markHandler.MoveMark(newLocation, timeArrivalMs);
        }
        else
        {
            _instance.MoveNonPlayerCharacter(charId, /*oldLocation,*/ newLocation, timeArrivalMs);
        }
    }

    public void DestroyExternally(GameObject objectInstance)
    {
        Destroy(objectInstance);
    }

    public GameObject InstantiateExternally(GameObject prefab, Transform parentTransform)
    {
        return Instantiate(prefab, parentTransform);
    }

    private void HandleGroundSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0)) //left click
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(camRay, out hit, _camRayDistance))
            {
                Vector3 hitPosition = hit.point;
                GameObject hitGameObject = hit.transform.gameObject;

                if (hitGameObject.tag.Equals("Ground") || hitGameObject.tag.Equals("Platform"))
                {
                    Instantiate(_groundSelectionMarkPrefab, hitPosition, Quaternion.identity);
                    MovePlayerCharacter(hitPosition);
                }
            }
        }
    }

    private void MoveNonPlayerCharacter(int charId, /*Vector3 oldPosition,*/  Vector3 newPosition, int timeArrivalMs)
    {
        GameObject character = null;
        LocalCharacterHandler charHandler = null;
        LocalCharacterDetails details = null;
        
        foreach (GameObject item in _localCharacterObjectList)
        {
            charHandler = item.GetComponent<LocalCharacterHandler>();
            details = charHandler.GetDetails();

            if (details.CharId == charId)
            {
                character = item;
                break;
            }
        }

        if (character == null)
        {
            Debug.Log($"Local place manager - MoveCharacterAsync() - cannot find character by char ID [{charId}]");
            return;
        }

        if (charHandler.IsPlayerMainCharacter)
        {
            Debug.Log($"Local place manager - MoveCharacterAsync() - cannot move player main character. Use MovePlayerCharacter() instead!");
            return;
        }

        Vector3 oldPosition = character.transform.position;
        float angle = Measure.CalculateMovingAngle(oldPosition, newPosition);

        charHandler.UpdateMovement
        (
            LocalCharacterHandler.MovementType.Moving,
            //oldPosition,
            newPosition,
            angle,
            timeArrivalMs,
            false
        );
    }

    private void MovePlayerCharacter(Vector3 newPosition)
    {
        if (_playerCharacter == null)
        {
            _chat.UpdateLog("MovePlayerCharacter() - cannot find player character instance");
            return;
        }

        LocalCharacterHandler charHandler = _playerCharacter.GetComponent<LocalCharacterHandler>();
        int timeArrivalMs = Measure.CalculateTimeArrivalInMs(_playerCharacter.transform.position, newPosition, charHandler.MovingVelocity);
        float angle = Measure.CalculateMovingAngle(_playerCharacter.transform.position, newPosition);
        
        charHandler.UpdateMovement
        (
            LocalCharacterHandler.MovementType.Moving, 
            /*_playerCharacter.transform.position,*/
            newPosition,
            angle,
            timeArrivalMs,
            true
        );
    }

    private async void WaitForServerDataAndLoadScene()
    {
        do
        {
            await Task.Factory.StartNew(() => Thread.Sleep(1000));
        }
        while (!_gameStateDetails.LocalTerrainDetailsListConfirmed || !_gameStateDetails.LocalCharacterDetailsListConfirmed);

        ReloadScene();
    }
    
    private void ReloadScene()
    {
        try
        {
            ReloadTerrain();
            ReloadCharacters();
            ReloadBoundingBox();
            HideLoadingScreen();
        }
        catch (Exception exception)
        {
            MainGameHandler.ShowMessageBox($"Failed to reload local scene: {exception.Message} | {exception.StackTrace}");
        }
    }

    public void ShowServerCollisionBoxes(bool show)
    {
        foreach (GameObject terrainSegment in _terrainSegmentObjectList)
        {
            terrainSegment.GetComponent<TerrainSegmentHandler>().ActivateServerColliderBox(show);
        }
    }

    private void AddNewNonPlayerCharacter(LocalCharacterDetails details)
    {
        GameObject createdObject = LocalCharacterCreator.CreateLocalCharacter(this, new StandardLocalCharacterBuilder(details, _gameStateDetails, _mainCamera));
        _localCharacterObjectList.Add(createdObject);
    }

    private void ReloadCharacters()
    {
        TppCameraManagerHandler cameraHandler = _tppCameraManager.GetComponent<TppCameraManagerHandler>();
        PlayerPositionMarkHandler positionMarkHandler = _playerPositionMark.GetComponent<PlayerPositionMarkHandler>();
        cameraHandler.SetAnchor(null);
        positionMarkHandler.SetPlayerCharacter(null);
        _playerCharacter = null;
        
        for (int i = _localCharacterObjectList.Count - 1; i >= 0; i--)
        {
            Destroy(_localCharacterObjectList[i]);
            _localCharacterObjectList.RemoveAt(i);
        }

        List<LocalCharacterDetails> detailsList = _gameStateDetails.GetLocalCharacterDetails();
        LocalCharacterHandler characterHandler;

        foreach (LocalCharacterDetails details in detailsList)
        {
            GameObject createdObject = LocalCharacterCreator.CreateLocalCharacter(this, new StandardLocalCharacterBuilder(details, _gameStateDetails, _mainCamera));
            characterHandler = createdObject.GetComponent<LocalCharacterHandler>();

            if (characterHandler.IsPlayerMainCharacter)
            {
                cameraHandler.SetAnchor(createdObject.transform);
                _playerCharacter = createdObject;
                _playerPositionMark.transform.position = _playerCharacter.transform.position;
                positionMarkHandler.SetPlayerCharacter(createdObject);
            }

            _localCharacterObjectList.Add(createdObject);
        }
    }

    private void ReloadTerrain()
    {
        for (int i = _terrainSegmentObjectList.Count - 1; i >= 0; i--)
        {
            Destroy(_terrainSegmentObjectList[i]);
            _terrainSegmentObjectList.RemoveAt(i);
        }

        List<TerrainDetails> detailsList = _gameStateDetails.GetLocalTerrainDetails();
        foreach (TerrainDetails details in detailsList)
        {
            GameObject createdObject = TerrainSegmentCreator.CreateTerrainSegment(this, new StandardTerrainSegmentBuilder(details));
            _terrainSegmentObjectList.Add(createdObject);
        }
    }

    private void ReloadBoundingBox()
    {
        LocalBoundingBoxHandler handler = _boundingBoxObject.GetComponent<LocalBoundingBoxHandler>();
        int maxBound = Mathf.Max
        (
            Convert.ToInt32(_gameStateDetails.LocalBound.x), 
            Convert.ToInt32(_gameStateDetails.LocalBound.y),
            Convert.ToInt32(_gameStateDetails.LocalBound.z)
        );
        handler.ReloadBoundingQuads(maxBound);
    }

    private void HideLoadingScreen()
    {
        LoadingPanelHandler handler = _loadingScreen.GetComponent<LoadingPanelHandler>();
        handler.Fade();
    }
}
