using System;

namespace BackgroundManagement.Models.ClientExchangeData
{
    public class WorldPlaceDataDetails : ICloneable
    {
        public int WorldPosX { get; set; }
        public int WorldPosY { get; set; }
        public int WmId { get; set; }
        public int PlaceType { get; set; }
        public string PlaceName { get; set; }

        public object Clone()
        {
            return new WorldPlaceDataDetails()
            {
                WorldPosX = this.WorldPosX,
                WorldPosY = this.WorldPosY,
                WmId = this.WmId,
                PlaceType = this.PlaceType,
                PlaceName = this.PlaceName
            };
        }
    }
}
