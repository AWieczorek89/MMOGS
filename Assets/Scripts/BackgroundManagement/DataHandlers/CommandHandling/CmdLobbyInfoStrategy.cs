using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using MMOC.BackgroundManagement;
using Newtonsoft.Json;
using System;
using UnityEngine.SceneManagement;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdLobbyInfoStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "lobbychar";
        private static IChat _chat;
        private string[] _cmdElements;
        private string _rawText;

        public CmdLobbyInfoStrategy(IChat chat)
        {
            _chat = chat;
        }

        public bool ValidateExecution(string command)
        {
            bool valid = false;
            
            _cmdElements = command.Split(' ');
            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(_keyWord, GlobalData.InputDataStringComparison))
            {
                valid = true;
                _rawText = command;
            }

            return valid;
        }

        public bool Execute()
        {
            bool executed = false;

            try
            {
                LobbyCharactersHandler handler = MainGameHandler.GetLobbyCharactersHandler();
                if (handler == null)
                    throw new Exception("lobby char. handler is NULL!");

                if (_cmdElements.Length == 2 && _cmdElements[1].Equals("endlist", GlobalData.InputDataStringComparison))
                {
                    handler.ListConfirmed = true;

                    if (MainGameHandler.CheckIfSceneActive(MainGameHandler.SceneType.CharLobby, SceneManager.GetActiveScene()))
                        CharacterListPanelHandler.Reload();
                    
                    MainGameHandler.HideLoadingScreen();
                    executed = true;
                }
                else
                if (_cmdElements.Length > 2 && _cmdElements[1].Equals("position", GlobalData.InputDataStringComparison))
                {
                    string discardedText = $"{_keyWord} position";
                    LobbyCharDetails charDetails = JsonConvert.DeserializeObject<LobbyCharDetails>(_rawText.Substring(discardedText.Length));
                    handler.Add(charDetails);
                    executed = true;
                }
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Lobby char. command execution error: {exception.Message}");
            }

            return executed;
        }
    }
}
