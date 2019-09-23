using MMOGS.Measurement.Units;

namespace MMOGS.Models.ClientExchangeData
{
    public class LocalCharacterDetails
    {
        public string Name { get; set; }
        public int HairstyleId { get; set; }
        public int CharId { get; set; }
        public string Action { get; set; }
        public string State { get; set; }
        public double Angle { get; set; }
        public double Velocity { get; set; }
        public string MovingStartTime { get; set; }
        public string MovingEndTime { get; set; }
        public Point3<double> StartingLoc { get; set; }
        public Point3<double> DestinationLoc { get; set; }
        public Point3<double> CurrentLoc { get; set; }
        public Point2<int> CurrentWorldLoc { get; set; }
        public string ModelCode { get; set; }
    }
}
