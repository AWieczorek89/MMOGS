using MMOGS.Interfaces;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.GameState;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CmdChatStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "msg";

        private ILogger _logger;
        private ChatHandler _chatHandler;
        private string _rawText = String.Empty;

        public CmdChatStrategy(ILogger logger, ChatHandler chatHandler)
        {
            _logger = logger ?? throw new Exception("Chat strategy - logger cannot be NULL!");
            _chatHandler = chatHandler ?? throw new Exception("Chat strategy - chat handler cannot be NULL!");
        }

        public bool ValidateExecution(ClientCommandInfo cmdInfo)
        {
            bool valid = false;
            
            if 
            (
                cmdInfo.CommandTxt.Length >= _keyWord.Length && 
                cmdInfo.CommandTxt.Substring(0, _keyWord.Length).Equals(_keyWord, GlobalData.InputDataStringComparison)
            )
            {
                _rawText = cmdInfo.CommandTxt;
                valid = true;
            }

            return valid;
        }

        public bool Execute(PlayerDetails playerDetails)
        {
            ExecuteAsync(playerDetails);
            return true;
        }

        private async void ExecuteAsync(PlayerDetails playerDetails)
        {
            try
            {
                string text = _rawText;
                ChatMessageDetails details = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<ChatMessageDetails>(text.Substring(_keyWord.Length)));
                if (details == null)
                    throw new Exception($"cannot deserialize JSON element [{text.Substring(_keyWord.Length)}]");

                _chatHandler.AddMessage(playerDetails.CharId, details);
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Cannot execute chat command: {exception.Message}");
            }
        }
    }
}
