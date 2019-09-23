namespace MMOGS.Models.Database
{
    public class DbWorldMapData
    {
        public int WmId { get; private set; } = -1;
        public int WorldPosX { get; private set; } = -1;
        public int WorldPosY { get; private set; } = -1;
        public int PlaceType { get; private set; } = -1;
        public bool IsRandomPlace { get; private set; } = true;
        public string PlaceName { get; private set; } = "";

        public DbWorldMapData
        (
            int wmId,
            int worldPosX,
            int worldPosY,
            int placeType,
            bool isRandomPlace,
            string placeName
        )
        {
            this.WmId = wmId;
            this.WorldPosX = worldPosX;
            this.WorldPosY = worldPosY;
            this.PlaceType = placeType;
            this.IsRandomPlace = isRandomPlace;
            this.PlaceName = placeName;
        }
    }
}
