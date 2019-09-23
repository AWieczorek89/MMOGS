using BackgroundManagement.GameHelpers.LocalCharacterBuilding;
using BackgroundManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationPanelHandler : MonoBehaviour
{
    public GameObject _characterAnchor;

    public InputField _nameInputField;
    public Dropdown _modelDropdown;
    public Dropdown _hairstyleDropdown;

    private static readonly string _noneSelectedText = "None selected";
    private Dictionary<string, Tuple<string, Type>> _modelCodeDataDictionary = null;

    private Text _modelDropdownText;
    private Text _hairstyleDropdownText;

    private void Start()
    {
        _modelCodeDataDictionary = StandardLocalCharacterBuilder.ModelCodeDataDictionary;
        _modelDropdownText = _modelDropdown.transform.Find("Label").GetComponent<Text>();
        _hairstyleDropdownText = _hairstyleDropdown.transform.Find("Label").GetComponent<Text>();

        ReloadPanel();
    }

    public void ReloadPanel()
    {
        _nameInputField.text = String.Empty;
        
        ReloadModelCodeList();
        ReloadHairstyleList();
    }
    
    private void ReloadModelCodeList()
    {
        _modelDropdown.options.Clear();

        foreach (var item in _modelCodeDataDictionary)
            _modelDropdown.options.Add(new Dropdown.OptionData() { text = item.Key });

        if (_modelCodeDataDictionary.Count > 0)
        {
            _modelDropdown.value = 0;
            _modelDropdownText.text = _modelCodeDataDictionary.ElementAt(0).Key;
        }  
        else
        {
            _modelDropdownText.text = _noneSelectedText;
        }
    }

    private void ReloadHairstyleList()
    {
        _hairstyleDropdown.options.Clear();
        //_hairstyleDropdown.transform.Find("Label").GetComponent<Text>().text = _noneSelectedText;
        
    }
    
    public void OnCancelButtonClick()
    {
        CharLobbySceneManagerHandler _sceneHandler = CharLobbySceneManagerHandler.GetLastInstance();
        if (_sceneHandler == null)
        {
            MainGameHandler.ShowMessageBox("Error #2: cannot find char lobby scene manager handler!");
            return;
        }

        _sceneHandler.ChangeLobbyState(CharLobbySceneManagerHandler.LobbyState.CharacterList);
    }
}
