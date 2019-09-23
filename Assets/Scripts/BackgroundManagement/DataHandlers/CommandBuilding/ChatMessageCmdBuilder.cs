using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.Commands;
using Newtonsoft.Json;
using System;

namespace BackgroundManagement.DataHandlers.CommandBuilding
{
    public class ChatMessageCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "msg";
        
        private CommandDetails _commandDetails = new CommandDetails();
        private string _rawText = "";

        public ChatMessageCmdBuilder(string command)
        {
            _rawText = command;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(_keyWord);
        }

        public void AddCommandElements()
        {
            try
            {
                string to = "";
                string message = _rawText;

                int cutIndex = -1;

                if (_rawText.Length >= 2 && _rawText.Substring(0, 2).Equals("/\"", StringComparison.InvariantCultureIgnoreCase))
                {
                    string cutSection = _rawText.Substring(2);
                    cutIndex = cutSection.IndexOf('"');

                    if (cutIndex > -1)
                    {
                        to = cutSection.Substring(0, cutIndex);
                        message = cutSection.Substring(cutIndex + 1);
                    }
                }

                if (message.Length >= 1 && message[0] == ' ')
                    message = message.Substring(1);

                ChatMessageDetails details = new ChatMessageDetails()
                {
                    IsPrivate = !String.IsNullOrWhiteSpace(to),
                    From = "",
                    To = to,
                    Message = message
                };

                _commandDetails.CommandElementList.Add(JsonConvert.SerializeObject(details));
            }
            catch (Exception exception)
            {
                MainGameHandler.ShowMessageBox($"Message building error: {exception.Message}");
            }
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
