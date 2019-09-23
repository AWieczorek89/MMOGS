using BackgroundManagement.Measurement.Units;

namespace BackgroundManagement.Models.ClientExchangeData
{
    public class CharacterPositionUpdateDetails
    {
        public string CreatedAt { get; set; }
        public string MovementType { get; set; }
        public int CharId { get; set; }
        public Point3<double> OldLocationLocal { get; set; }
        public Point3<double> NewLocationLocal { get; set; }
        public int TimeArrivalMsLocal { get; set; }
        public Point2<int> OldLocationWorld { get; set; }
        public Point2<int> NewLocationWorld { get; set; }
    }
}
