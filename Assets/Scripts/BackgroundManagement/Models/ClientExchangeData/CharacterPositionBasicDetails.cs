using BackgroundManagement.Measurement.Units;

namespace BackgroundManagement.Models.ClientExchangeData
{
    public class CharacterPositionBasicDetails
    {
        public int CharId { get; set; }
        public int WmId { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public Point3<int> LocalBound { get; set; }
        public bool IsOnWorldMap { get; set; }
        public Point3<float> Position { get; set; }
    }
}
