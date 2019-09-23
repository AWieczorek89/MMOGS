using BackgroundManagement.Models.ClientExchangeData;
using System.Collections.Generic;

namespace BackgroundManagement.DataHandlers
{
    public class LobbyCharactersHandler
    {
        private readonly object _loadingLock = new object();
        private readonly object _dataLock = new object();
        
        private bool _listConfirmed = false;
        public bool ListConfirmed
        {
            get { lock (_loadingLock) { return _listConfirmed; } }
            set { lock (_loadingLock) { _listConfirmed = value; } }
        }

        private List<LobbyCharDetails> _lobbyCharList = new List<LobbyCharDetails>();

        public LobbyCharactersHandler()
        {
        }

        public void Add(LobbyCharDetails position)
        {
            bool found = false;
            
            lock (_dataLock)
            {
                for (int i = 0; i < _lobbyCharList.Count; i++)
                {
                    if (_lobbyCharList[i].CharId == position.CharId)
                    {
                        _lobbyCharList[i] = position;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    _lobbyCharList.Add(position);
            }
        }

        public List<LobbyCharDetails> GetAll()
        {
            List<LobbyCharDetails> resultList = new List<LobbyCharDetails>();

            lock (_dataLock)
            {
                resultList = _lobbyCharList.Clone();
            }

            return resultList;
        }
        
        public void Clear()
        {
            lock (_dataLock)
            {
                _lobbyCharList.Clear();
            }
        }
    }
}
