using System.Collections.Generic;

namespace BackgroundManagement.Models.Chat
{
    public class ChatPackage
    {
        public List<string> GlobalChatMessageList { get; set; } = new List<string>();
        public List<string> PrivateChatMessageList { get; set; } = new List<string>();
        public List<string> LogMessageList { get; set; } = new List<string>();
    }
}
