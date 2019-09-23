using BackgroundManagement.DataHandlers;
using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LoginButtonHandler : MonoBehaviour
{
    public InputField _loginInputField;
    public InputField _passInputField;
    public Text _loginButtonText;

    private string _loginButtonDefaultText;
    private float _timeCounter = 0f;

    void Start()
    {
        _loginButtonDefaultText = _loginButtonText.text;
    }

    void Update()
    {
        _timeCounter += Time.deltaTime;

        if (_timeCounter > 0.5f)
        {
            _timeCounter = 0f;

            switch (MainGameHandler.CheckLoginState())
            {
                case ConnectionChecker.LoginState.WaitingForResponse:
                    _loginButtonText.text = "Logging in...";
                    break;
                default:
                    _loginButtonText.text = _loginButtonDefaultText;
                    break;
            }
        }
    }

    public void ActivateLogin()
    {
        ConnectionChecker connChecker = MainGameHandler.GetConnectionChecker();
        if (connChecker == null)
        {
            MainGameHandler.ShowMessageBox("Login button handler - ActivateLogin() - no reference to connection checker!", "Critical error", null);
            return;
        }

        ConnectionChecker.LoginState loginState = connChecker.ClientLoginState;
        if (loginState == ConnectionChecker.LoginState.WaitingForResponse)
            return;

        string login = _loginInputField.text;
        string pass = _passInputField.text;

        if (String.IsNullOrWhiteSpace(login))
        {
            MainGameHandler.ShowMessageBox("Your login cannot be empty!");
            return;
        }

        connChecker.SetLoginState(ConnectionChecker.LoginState.WaitingForResponse);
        CommandHandler.Send(new LoginRequestCmdBuilder(login, pass));
    }
}
