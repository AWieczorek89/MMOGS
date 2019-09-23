using MMOGS;
using MMOGS.Interfaces;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;

namespace MMOGS.DataHandlers
{
    public class PlayerHandler : IDisposable
    {
        ILogger _logger;
        private List<PlayerDetails> _playerDetailsList = new List<PlayerDetails>();

        private readonly object _detailsListLock = new object();

        public PlayerHandler(ILogger logger)
        {
            _logger = logger ?? throw new Exception("Player handler - logger cannot be NULL!");
        }

        /// <summary>
        /// Gets current character (char_id) of player
        /// </summary>
        public int GetPlayerCharacterId(int tcpClientId)
        {
            int charId = -1;

            lock (_detailsListLock)
            {
                foreach (PlayerDetails details in _playerDetailsList)
                {
                    if (details.TcpClientId == tcpClientId)
                    {
                        charId = details.CharId;
                        break;
                    }
                }
            }

            return charId;
        }

        /// <summary>
        /// Gets current character (char_id) of player
        /// </summary>
        public int GetPlayerCharacterId(string login)
        {
            int charId = -1;

            lock (_detailsListLock)
            {
                foreach (PlayerDetails details in _playerDetailsList)
                {
                    if (details.Login.ToLower().Equals(login.ToLower(), GlobalData.InputDataStringComparison))
                    {
                        charId = details.CharId;
                        break;
                    }
                }
            }

            return charId;
        }

        /// <summary>
        /// Sets current character (char_id) of player
        /// </summary>
        public bool SetPlayerCharacter(int charId, string login)
        {
            bool success = false;

            lock (_detailsListLock)
            {
                foreach (PlayerDetails details in _playerDetailsList)
                {
                    if (details.Login.ToLower().Equals(login.ToLower(), GlobalData.InputDataStringComparison))
                    {
                        details.CharId = charId;
                        success = true;
                        break;
                    }
                }
            }

            return success;
        }

        public bool CheckIfLoginInUse(string login)
        {
            bool isInUse = false;
            if (String.IsNullOrWhiteSpace(login))
                return isInUse;

            lock (_detailsListLock)
            {
                foreach (PlayerDetails details in _playerDetailsList)
                {
                    if (details.Login.ToLower().Equals(login.ToLower(), GlobalData.InputDataStringComparison))
                    {
                        isInUse = true;
                        break;
                    }
                }
            }

            return isInUse;
        }

        public List<PlayerDetails> GetAllPlayers()
        {
            List<PlayerDetails> playerDetailsList = null;

            lock (_detailsListLock)
            {
                playerDetailsList = _playerDetailsList.Clone();
            }

            return playerDetailsList;
        }
        
        public PlayerDetails GetPlayerByCurrentCharId(int currentMainCharId)
        {
            PlayerDetails playerDetails = null;
            if (currentMainCharId < 0)
                return null;

            lock (_detailsListLock)
            {
                foreach (PlayerDetails details in _playerDetailsList)
                {
                    if (details.CharId == currentMainCharId)
                    {
                        playerDetails = details;
                        break;
                    }
                }
            }

            return playerDetails;
        }

        public PlayerDetails GetPlayer(int tcpClientId)
        {
            PlayerDetails playerDetails = null;

            lock (_detailsListLock)
            {
                foreach (PlayerDetails details in _playerDetailsList)
                {
                    if (details.TcpClientId == tcpClientId)
                    {
                        playerDetails = details;
                        break;
                    }
                }
            }

            return playerDetails;
        }

        public void UnregisterPlayer(int tcpClientId)
        {
            string msg = "";
            bool found = false;

            lock (_detailsListLock)
            {
                for (int i = _playerDetailsList.Count - 1; i >= 0; i--)
                {
                    if (_playerDetailsList[i].TcpClientId == tcpClientId)
                    {
                        found = true;
                        msg = $"Unregistered existing player, TCP client ID [{tcpClientId}]";
                        _playerDetailsList.RemoveAt(i);
                        break;
                    }
                }
            }

            if (!found)
                msg = $"Cannot unregister player with TCP client ID [{tcpClientId}] - not found!";

            _logger.UpdateLog(msg);
        }

        public PlayerDetails RegisterPlayer(int tcpClientId)
        {
            PlayerDetails playerDetails = null;
            string msg = "";

            lock (_detailsListLock)
            {
                foreach (PlayerDetails details in _playerDetailsList)
                {
                    if (details.TcpClientId == tcpClientId)
                    {
                        playerDetails = details;
                        break;
                    }
                }

                if (playerDetails != null)
                {
                    msg = $"Player registration failed - player with TCP client ID [{tcpClientId}] already exists!";
                }
                else
                {
                    playerDetails = new PlayerDetails(tcpClientId);
                    _playerDetailsList.Add(playerDetails);
                    msg = $"Registered new player, TCP client ID [{tcpClientId}]";
                }
            }

            _logger.UpdateLog(msg);
            return playerDetails;
        }

        public void Dispose()
        {
            lock (_detailsListLock)
            {
                _playerDetailsList.Clear();
            }
        }
    }
}
