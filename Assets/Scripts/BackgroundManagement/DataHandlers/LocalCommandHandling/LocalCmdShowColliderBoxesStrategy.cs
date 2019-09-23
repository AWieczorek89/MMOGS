using BackgroundManagement.Interfaces;
using MMOC.BackgroundManagement;
using System;
using UnityEngine;

namespace BackgroundManagement.DataHandlers.LocalCommandHandling
{
    public class LocalCmdShowColliderBoxesStrategy : ILocalCommandHandlingStrategy
    {
        private static readonly string _keyWord = "colliderbox";

        private IChat _chat;
        private string[] _cmdElements;

        public LocalCmdShowColliderBoxesStrategy(IChat chat)
        {
            _chat = chat;
        }

        public bool ValidateExecution(string command)
        {
            bool valid = false;

            _cmdElements = command.Split(' ');
            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(_keyWord, GlobalData.InputDataStringComparison))
                valid = true;

            return valid;
        }

        public bool Execute()
        {
            bool executed = false;

            try
            {
                if (_cmdElements.Length != 2)
                    throw new Exception($"wrong count of elements [{_cmdElements.Length}]");

                bool show = false;

                if (_cmdElements[1].Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    show = true;
                }
                else
                if (_cmdElements[1].Equals("false", StringComparison.InvariantCultureIgnoreCase))
                {
                    show = false;
                }
                else
                {
                    throw new Exception($"incorrect command element [{_cmdElements[1]}]. Must be 'true' or 'false'!");
                }

                if (!MainGameHandler.CheckIfSceneActive(MainGameHandler.SceneType.LocalPlace, MainGameHandler.CurrentScene))
                    throw new Exception("local place scene is not active!");

                GameObject[] gameObjects = MainGameHandler.CurrentScene.GetRootGameObjects();
                LocalPlaceSceneManagerHandler handler = null;

                foreach (GameObject obj in gameObjects)
                {
                    if (obj.name.Equals("LocalPlaceSceneManager", StringComparison.InvariantCultureIgnoreCase))
                    {
                        handler = obj.GetComponent<LocalPlaceSceneManagerHandler>();
                        break;
                    }
                }

                if (handler == null)
                    throw new Exception("cannot find scene manager handler!");

                handler.ShowServerCollisionBoxes(show);
                executed = true;
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Cannot execute command [{_keyWord}]: {exception.Message}");
            }

            return executed;
        }
    }
}
