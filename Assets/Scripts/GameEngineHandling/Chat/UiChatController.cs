using BackgroundManagement.DataHandlers.LocalCommandHandling;
using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.Chat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiChatController : MonoBehaviour
{
    public enum MessageType
    {
        Global,
        Private,
        Log
    }

    public InputField _msgInput;
    public GameObject _globalChatTabPanel;
    public GameObject _privateChatTabPanel;
    public GameObject _logTabPanel;
    public GameObject _globalChatContent;
    public GameObject _privateChatContent;
    public GameObject _logContent;
    public GameObject _textElement; //prefab

    private static IChat _chat;
    private static readonly float _chatRefreshInterval = 1f;
    private static float _chatRefreshCounter = 0f;
    private MessageType _currentType = MessageType.Global;
    
    private List<GameObject> _globalChatTextElements = new List<GameObject>();
    private List<GameObject> _privateChatTextElements = new List<GameObject>();
    private List<GameObject> _logTextElements = new List<GameObject>();
    
	void Start ()
    {
        _chat = MainGameHandler.GetChatHandler();
	}
    
    void Update()
    {
        HandleMsgSending();

        _chatRefreshCounter += Time.deltaTime;

        if (_chatRefreshCounter >= _chatRefreshInterval)
        {
            _chatRefreshCounter = 0f;
            GetNewMessagesAsync();
        }
    }

    private void HandleMsgSending()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameObject obj = EventSystem.current.currentSelectedGameObject;

            if (obj != null && obj.name.Equals("MsgInputField", StringComparison.InvariantCultureIgnoreCase))
            {
                string msgCommand = _msgInput.text;

                if (!String.IsNullOrWhiteSpace(msgCommand))
                {
                    LocalCommandHandler cmdHandler = MainGameHandler.GetLocalCommandHandler();
                    cmdHandler.ExecuteCommand(msgCommand);
                    _msgInput.text = "";
                }
            }
        }
    }
    
    private void ShowPanel(MessageType type)
    {
        _globalChatTabPanel.SetActive(type == MessageType.Global);
        _privateChatTabPanel.SetActive(type == MessageType.Private);
        _logTabPanel.SetActive(type == MessageType.Log);
        
        _currentType = type;
    }
    
    private void ScrollDown(MessageType type)
    {
        GameObject currentPanel = _globalChatTabPanel;
        if (type == MessageType.Private) currentPanel = _privateChatTabPanel;
        else if (type == MessageType.Log) currentPanel = _logTabPanel;

        currentPanel.transform.Find("ChatScrollView").gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    private async void GetNewMessagesAsync()
    {
        ChatPackage msgPack = await _chat.GetMessagesTaskStart
        (
            (_currentType == MessageType.Global),
            (_currentType == MessageType.Private),
            (_currentType == MessageType.Log)
        );

        if (msgPack.GlobalChatMessageList.Count > 0)
            UpdateChat(msgPack.GlobalChatMessageList, MessageType.Global);

        if (msgPack.PrivateChatMessageList.Count > 0)
            UpdateChat(msgPack.PrivateChatMessageList, MessageType.Private);

        if (msgPack.LogMessageList.Count > 0)
            UpdateChat(msgPack.LogMessageList, MessageType.Log);
    }

    public void OnGlobalButtonClick()
    {
        ShowPanel(MessageType.Global);
    }

    public void OnPrivateButtonClick()
    {
        ShowPanel(MessageType.Private);
    }

    public void OnLogButtonClick()
    {
        ShowPanel(MessageType.Log);
    }

    private static int testCounter = 0;
    public void OnTestButtonClick()
    {
        _chat.UpdateGlobalChat($"jakiś tekst {++testCounter}", "testowy autor");
        _chat.UpdatePrivateChat($"test priv {testCounter}", "od", "do");
        _chat.UpdateLog($"log {testCounter}");
        _chat.UpdateLog($"log2 {testCounter}");
    }

    private void UpdateChat(List<string> messageList, MessageType type)
    {
        if (messageList.Count == 0)
            return;
        
        List<GameObject> currentList = new List<GameObject>();
        Text text;

        switch (type)
        {
            case MessageType.Global:
                currentList = _globalChatTextElements;
                break;
            case MessageType.Private:
                currentList = _privateChatTextElements;
                break;
            case MessageType.Log:
                currentList = _logTextElements;
                break;
        }

        if (messageList.Count != currentList.Count)
            ResizeTextContent(currentList, messageList.Count, type);

        for (int i = 0; i < messageList.Count; i++)
        {
            if (currentList[i] == null)
                continue;

            text = currentList[i].GetComponent<Text>();
            text.text = messageList[i];
        }
    }

    private void ResizeTextContent(List<GameObject> currentList, int newSize, MessageType type)
    {
        if (newSize <= 0)
            return;
        
        int difference = newSize - currentList.Count;
        GameObject newTextObject;
        Transform parentTransform = _globalChatContent.transform;
        
        switch (type)
        {
            case MessageType.Global:
                parentTransform = _globalChatContent.transform;
                break;
            case MessageType.Private:
                parentTransform = _privateChatContent.transform;
                break;
            case MessageType.Log:
                parentTransform = _logContent.transform;
                break;
        }

        if (difference > 0)
        {
            for (int i = 0; i < difference; i++)
            {
                newTextObject = Instantiate(_textElement, parentTransform);
                currentList.Add(newTextObject);
            }
        }
        else
        if (difference < 0)
        {
            int deleteCounter = difference;

            do
            {
                Destroy(currentList[currentList.Count - 1]);
                currentList.RemoveAt(currentList.Count - 1);
                deleteCounter++;
            }
            while (deleteCounter < 0);
        }
    }
}
