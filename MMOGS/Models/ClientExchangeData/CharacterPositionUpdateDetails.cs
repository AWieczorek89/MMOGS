using MMOGS.Measurement.Units;
using System;

namespace MMOGS.Models.ClientExchangeData
{
    public class CharacterPositionUpdateDetails
    {
        public string CreatedAt { get; private set; }
        public string MovementType { get; set; }
        public int CharId { get; set; }
        public Point3<double> OldLocationLocal { get; set; }
        public Point3<double> NewLocationLocal { get; set; }
        public int TimeArrivalMsLocal { get; set; }
        public Point2<int> OldLocationWorld { get; set; }
        public Point2<int> NewLocationWorld { get; set; }

        public CharacterPositionUpdateDetails()
        {
            this.CreatedAt = DateTime.Now.ToString(GlobalData.MovementTimeExchangeFormat);
        }
    }
}
