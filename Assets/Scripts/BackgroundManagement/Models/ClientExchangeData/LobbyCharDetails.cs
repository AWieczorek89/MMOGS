using System;

namespace BackgroundManagement.Models.ClientExchangeData
{
    public class LobbyCharDetails : ICloneable
    {
        public int CharId { get; set; }
        public string Name { get; set; }
        public string ModelCode { get; set; }
        public int HairstyleId { get; set; }

        public object Clone()
        {
            return new LobbyCharDetails()
            {
                CharId = this.CharId,
                Name = this.Name,
                ModelCode = this.ModelCode,
                HairstyleId = this.HairstyleId
            };
        }
    }
}
