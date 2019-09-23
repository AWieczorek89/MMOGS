using BackgroundManagement.DataHandlers;
using BackgroundManagement.Models.ClientExchangeData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListPanelHandler : MonoBehaviour
{
    public GameObject _scrollViewContent;

    private List<LobbyCharDetails> _lobbyCharDetailsList = new List<LobbyCharDetails>();
    private List<GameObject> _charUiPositionList = new List<GameObject>();

    private static CharacterListPanelHandler _lastInstance = null;
    private static readonly float _rectTransformMargin = 5f;

    void Start ()
    {
        _lastInstance = this;
	}
    
    public static void Reload()
    {
        if (_lastInstance == null)
            return;
        
        try
        {
            LobbyCharactersHandler charHandler = MainGameHandler.GetLobbyCharactersHandler();
            _lastInstance._lobbyCharDetailsList = charHandler.GetAll();
            _lastInstance.ReloadCharactersPosition();
        }
        catch (Exception exception)
        {
            Debug.Log($"CharacterListPanelHandler - Reload() - error: {exception.Message}");
        }
    }
    
    private void ReloadCharactersPosition()
    {
        try
        {
            ClearAllPositions();

            GameObject charUiPositionPrefab = (GameObject)Resources.Load("Prefabs/UI/CharLobby/CharPositionPanel", typeof(GameObject));
            RectTransform charUiPosRectTransform = charUiPositionPrefab.GetComponent<RectTransform>();
            RectTransform scrollViewRectTransform = _scrollViewContent.GetComponent<RectTransform>();
            
            scrollViewRectTransform.sizeDelta = new Vector2
            (
                scrollViewRectTransform.sizeDelta.x,
                (_lobbyCharDetailsList.Count * (charUiPosRectTransform.sizeDelta.y + _rectTransformMargin)) + _rectTransformMargin
            );

            LobbyCharDetails charDetails;
            GameObject newPosition;
            RectTransform newPosRectTransform;
            CharLobbyPositionHandler charLobbyPosHandler;
            float posY = -1 * (0.5f * charUiPosRectTransform.sizeDelta.y + _rectTransformMargin);
            float basicPosY = charUiPosRectTransform.position.y;
            float basicHeight = charUiPosRectTransform.sizeDelta.y;

            for (int i = 0; i < _lobbyCharDetailsList.Count; i++)
            {
                charDetails = _lobbyCharDetailsList[i];
                newPosition = Instantiate(charUiPositionPrefab, _scrollViewContent.transform);
                newPosRectTransform = newPosition.GetComponent<RectTransform>();
                charLobbyPosHandler = newPosition.GetComponent<CharLobbyPositionHandler>();

                charLobbyPosHandler.Set(charDetails.Name, charDetails.CharId, charDetails.ModelCode);
                
                newPosRectTransform.anchoredPosition = new Vector3
                (
                    newPosRectTransform.anchoredPosition.x,
                    posY
                );

                posY -= (charUiPosRectTransform.sizeDelta.y + _rectTransformMargin);
            }
        }
        catch (Exception exception)
        {
            Debug.Log($"CharacterListPanelHandler - ReloadCharacterPositions() - error: {exception.Message}");
        }
    }

    private void ClearAllPositions()
    {
        for (int i = _charUiPositionList.Count - 1; i >= 0; i--)
        {
            Destroy(_charUiPositionList[i]);
            _charUiPositionList.RemoveAt(i);
        }
    }

    public void OnNewCharacterButtonClick()
    {
        CharLobbySceneManagerHandler _sceneHandler = CharLobbySceneManagerHandler.GetLastInstance();
        if (_sceneHandler == null)
        {
            MainGameHandler.ShowMessageBox("Error #1: cannot find char lobby scene manager handler!");
            return;
        }

        _sceneHandler.ChangeLobbyState(CharLobbySceneManagerHandler.LobbyState.CharacterCreation);
    }
}
