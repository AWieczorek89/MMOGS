using BackgroundManagement.Interfaces;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxHandler : MonoBehaviour
{
    public GameObject _titleText;
    public GameObject _contentText;

    private IChat _chat;
    private Action _actionAfterConfirmation = null;

    void Start ()
    {
        _chat = MainGameHandler.GetChatHandler();
	}
	
    public void Set(string content, string title = "", Action actionAfterConfirmation = null)
    {
        try
        {
            Text contentTxtObject = _contentText.GetComponent<Text>();
            Text titleTxtObject = _titleText.GetComponent<Text>();

            contentTxtObject.text = content;

            if (!String.IsNullOrWhiteSpace(title))
                titleTxtObject.text = title;

            _actionAfterConfirmation = actionAfterConfirmation;
        }
        catch (Exception exception)
        {
            _chat.UpdateLog($"Message box setting error: {exception.Message}");
        }
    }

    public void OnOkButtonClick()
    {
        Destroy(this.gameObject);
        _actionAfterConfirmation?.Invoke();
    }
}
