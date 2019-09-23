using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using MMOC.BackgroundManagement;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BackgroundManagement.DataHandlers.CommandHandling
{
    public class CmdChatMessageStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "msg";

        private IChat _chat;
        private string _rawText = String.Empty;

        public CmdChatMessageStrategy(IChat chat)
        {
            _chat = chat;
        }

        public bool ValidateExecution(string command)
        {
            bool valid = false;

            if
            (
                command.Length >= _keyWord.Length &&
                command.Substring(0, _keyWord.Length).Equals(_keyWord, GlobalData.InputDataStringComparison)
            )
            {
                _rawText = command;
                valid = true;
            }

            return valid;
        }

        public bool Execute()
        {
            ExecuteAsync();
            return true;
        }

        private async void ExecuteAsync()
        {
            try
            {
                ChatMessageDetails details = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<ChatMessageDetails>(_rawText.Substring(_keyWord.Length)));
                if (details == null)
                    throw new Exception($"cannot deserialize JSON element [{_rawText.Substring(_keyWord.Length)}]");

                if (details.IsPrivate)
                {
                    _chat.UpdatePrivateChat(details.Message, details.From, details.To);
                }
                else
                {
                    _chat.UpdateGlobalChat(details.Message, details.From);
                }
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Cannot execute chat message command: {exception.Message}");
            }
        }
    }
}
