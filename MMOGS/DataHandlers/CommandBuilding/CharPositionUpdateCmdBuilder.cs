using MMOGS.Interfaces;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.Commands;
using Newtonsoft.Json;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class CharPositionUpdateCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "charposupdate";

        private CommandDetails _commandDetails = new CommandDetails();
        private CharacterPositionUpdateDetails _details;

        public CharPositionUpdateCmdBuilder(CharacterPositionUpdateDetails details)
        {
            _details = details ?? throw new Exception("CharPositionUpdateCmdBuilder - detail object cannot be NULL!");
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(CharPositionUpdateCmdBuilder._keyWord);
        }

        public void AddCommandElements()
        {
            _commandDetails.CommandElementList.Add(JsonConvert.SerializeObject(_details));
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
