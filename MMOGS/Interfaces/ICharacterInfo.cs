using MMOGS.Measurement.Units;
using MMOGS.Models;
using MMOGS.Models.GameState;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMOGS.Interfaces
{
    public interface ICharacterInfo
    {
        
        CharacterData GetCharacterByName(string charName);
        CharacterData GetCharacterById(int charId);
        List<CharacterData> GetCharactersById(IList<int> charIdList);
        List<CharacterData> GetCharactersByAccId(int accId);
        List<CharacterData> GetCharactersByWorldLocation(int wmId, bool isOnWorldMap, int parentObjectId);
        Task<CharacterData> GetCharacterByIdTaskStart(int charId);
        Task<CharacterData> GetCharacterByNameTaskStart(string charName);
        Task<List<CharacterData>> GetCharactersByIdTaskStart(IList<int> charIdList);
        Task<List<CharacterData>> GetCharactersByWorldLocationTaskStart(int wmId, bool isOnWorldMap, int parentObjectId);
    }
}
