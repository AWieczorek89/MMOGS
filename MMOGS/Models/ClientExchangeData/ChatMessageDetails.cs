using System;

namespace MMOGS.Models.ClientExchangeData
{
    public class ChatMessageDetails : ICloneable
    {
        public bool IsPrivate { get; set; } = false;
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public string Message { get; set; } = "";

        public object Clone()
        {
            return new ChatMessageDetails()
            {
                IsPrivate = this.IsPrivate,
                From = this.From,
                To = this.To,
                Message = this.Message
            };
        }
    }
}
