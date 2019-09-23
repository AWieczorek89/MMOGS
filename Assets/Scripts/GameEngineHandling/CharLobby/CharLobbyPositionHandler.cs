using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using UnityEngine;
using UnityEngine.UI;

public class CharLobbyPositionHandler : MonoBehaviour
{
    public Text _nameText;

    private int _charId = -1;
    private string _modelCode = "";
    
    public void Set(string name, int charId, string modelCode)
    {
        _nameText.text = name;
        _charId = charId;
        _modelCode = modelCode;
    }

	public void OnChooseButtonClick()
    {
        if (_charId < 0)
        {
            Debug.Log("CharLobbyPositionHandler - OnChooseButtonClick() - charId not set!");
            return;
        }

        CommandHandler.Send(new ChooseCharCmdBuilder(_charId));
    }
}
