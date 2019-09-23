namespace BackgroundManagement.Models.ClientExchangeData
{
    public class ChatMessageDetails
    {
        public bool IsPrivate { get; set; } = false;
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
