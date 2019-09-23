using MMOGS.Interfaces;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.Commands;
using Newtonsoft.Json;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class CharPositionBasicDetailsCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "positionbasic";

        private CommandDetails _commandDetails = new CommandDetails();
        private CharacterPositionBasicDetails _details;

        public CharPositionBasicDetailsCmdBuilder(CharacterPositionBasicDetails details)
        {
            _details = details ?? throw new Exception("CharPositionBasicDetailsCmdBuilder - detail object reference cannot be NULL!");
        }

        public void AddKeyword()
        {
            if (_commandDetails.CommandElementList.Count > 0)
                throw new Exception($"CharPositionBasicDetailsCmdBuilder - cannot add key word to command. Command list is not empty, count [{_commandDetails.CommandElementList.Count}]!");

            _commandDetails.CommandElementList.Add(CharPositionBasicDetailsCmdBuilder._keyWord);
        }

        public void AddCommandElements()
        {
            if (_commandDetails.CommandElementList.Count != 1)
                throw new Exception($"CharPositionBasicDetailsCmdBuilder - cannot add command elements. Wrong count of key words [{_commandDetails.CommandElementList.Count}]!");
            
            _commandDetails.CommandElementList.Add(JsonConvert.SerializeObject(_details));
        }

        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
