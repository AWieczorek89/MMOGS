using MMOGS.Interfaces;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.Commands;
using Newtonsoft.Json;
using System;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class WorldDetailsCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "worldplace";

        private CommandDetails _commandDetails = new CommandDetails();
        WorldPlaceDataDetails _details;
        bool _isListConfirmation = false;

        public WorldDetailsCmdBuilder(bool isListConfirmation, WorldPlaceDataDetails details = null)
        {
            if (!isListConfirmation && details == null)
                throw new Exception("WorldDetailsCmdBuilder - detail object cannot be NULL for no list confirmation!");

            _details = details;
            _isListConfirmation = isListConfirmation;
        }

        public void AddKeyword()
        {
            if (_commandDetails.CommandElementList.Count > 0)
                throw new Exception($"WorldDetailsCmdBuilder - cannot add key word to command. Command list is not empty, count [{_commandDetails.CommandElementList.Count}]!");

            _commandDetails.CommandElementList.Add(WorldDetailsCmdBuilder._keyWord);
        }

        public void AddCommandElements()
        {
            if (_commandDetails.CommandElementList.Count != 1)
                throw new Exception($"WorldDetailsCmdBuilder - cannot add command elements. Wrong count of key words [{_commandDetails.CommandElementList.Count}]!");

            if (_isListConfirmation)
            {
                _commandDetails.CommandElementList.Add("endlist");
            }
            else
            {
                _commandDetails.CommandElementList.Add("position");
                _commandDetails.CommandElementList.Add(JsonConvert.SerializeObject(_details));
            }
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
