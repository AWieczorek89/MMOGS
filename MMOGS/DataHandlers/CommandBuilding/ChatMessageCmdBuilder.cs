using MMOGS.Interfaces;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.Commands;
using Newtonsoft.Json;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class ChatMessageCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "msg";

        private CommandDetails _commandDetails = new CommandDetails();
        private ChatMessageDetails _details;

        public ChatMessageCmdBuilder(ChatMessageDetails details)
        {
            _details = details;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(_keyWord);
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
