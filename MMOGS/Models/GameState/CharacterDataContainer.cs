using MMOGS.Models.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMOGS.Models.GameState
{
    public class CharacterDataContainer : IDisposable
    {
        private List<CharacterData> _characterList = new List<CharacterData>();
        private readonly object _charListLock = new object();
        
        public CharacterDataContainer()
        {
        }

        public List<CharacterData> GetCharactersByWorldLocation(int wmId, bool isOnWorldMap, int parentObjectId)
        {
            List<CharacterData> charResultList = new List<CharacterData>();

            lock (_charListLock)
            {
                foreach (CharacterData charData in _characterList)
                {
                    if (charData.WmId == wmId && charData.IsOnWorldMap == isOnWorldMap)
                    {
                        if (isOnWorldMap)
                        {
                            charResultList.Add(charData);
                        }
                        else
                        if (charData.ParentObjectId == parentObjectId)
                        {
                            charResultList.Add(charData);
                        }
                    }
                }
            }

            return charResultList;
        }

        public List<CharacterData> GetCharactersByAccId(int accId)
        {
            List<CharacterData> charResultList = new List<CharacterData>();

            lock (_charListLock)
            {
                foreach (CharacterData charData in _characterList)
                {
                    if (charData.AccId == accId)
                    {
                        charResultList.Add(charData);
                    }
                }
            }

            return charResultList;
        }

        public List<CharacterData> GetCharactersById(IList<int> charIdList)
        {
            List<CharacterData> resultList = new List<CharacterData>();

            lock (_charListLock)
            {
                foreach (int charId in charIdList)
                {
                    foreach (CharacterData charData in _characterList)
                    {
                        if (charData.CharId == charId)
                        {
                            resultList.Add(charData);
                            break;
                        }
                    }
                }
            }

            return resultList;
        }

        public CharacterData GetCharacterById(int charId)
        {
            CharacterData result = null;

            lock (_charListLock)
            {
                foreach (CharacterData charData in _characterList)
                {
                    if (charData.CharId == charId)
                    {
                        result = charData;
                        break;
                    }
                }
            }

            return result;
        }

        public CharacterData GetCharacterByName(string charName)
        {
            CharacterData result = null;

            lock (_charListLock)
            {
                foreach (CharacterData charData in _characterList)
                {
                    if (charData.GetCharacterName().Equals(charName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = charData;
                        break;
                    }
                }
            }

            return result;
        }

        public Task<BoxedData> InitializeCharacterDataTaskStart(List<DbCharactersData> dbCharDataList)
        {
            var t = new Task<BoxedData>(() => InitializeCharacterData(dbCharDataList));
            t.Start();
            return t;
        }

        private BoxedData InitializeCharacterData(List<DbCharactersData> dbCharDataList)
        {
            BoxedData data = new BoxedData();
            bool success = false;
            string msg = "";

            try
            {
                ClearData();
                int charCounter = 0;
                
                lock (_charListLock)
                {
                    foreach (DbCharactersData dbCharData in dbCharDataList)
                    {
                        
                        CharacterData charData = new CharacterData(dbCharData);
                        _characterList.Add(charData);
                        charCounter++;
                    }
                }
                
                msg = $"Character data container - added [{charCounter}] chars.";
                success = true;
            }
            catch (Exception exception)
            {
                msg = $"Character data init. failed: {exception.Message} | {exception.StackTrace}";
            }

            data.Data = success;
            data.Msg = msg;
            return data;
        }

        ~CharacterDataContainer()
        {
            ClearData();
        }

        public void Dispose()
        {
            ClearData();
        }

        public void ClearData()
        {
            lock (_charListLock)
            {
                foreach (CharacterData charData in _characterList)
                    charData.Dispose();

                _characterList.Clear();
            }
        }
    }
}
