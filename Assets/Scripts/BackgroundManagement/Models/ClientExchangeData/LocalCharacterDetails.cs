using BackgroundManagement.Measurement.Units;
using System;

namespace BackgroundManagement.Models.ClientExchangeData
{
    public class LocalCharacterDetails : ICloneable
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

        public object Clone()
        {
            return new LocalCharacterDetails
            {
                Name = this.Name,
                HairstyleId = this.HairstyleId,
                CharId = this.CharId,
                Action = this.Action,
                State = this.State,
                Angle = this.Angle,
                Velocity = this.Velocity,
                MovingStartTime = this.MovingStartTime,
                MovingEndTime = this.MovingEndTime,
                StartingLoc = new Point3<double>
                (
                    this.StartingLoc.X,
                    this.StartingLoc.Y,
                    this.StartingLoc.Z
                ),
                DestinationLoc = new Point3<double>
                (
                    this.DestinationLoc.X,
                    this.DestinationLoc.Y,
                    this.DestinationLoc.Z
                ),
                CurrentLoc = new Point3<double>
                (
                    this.CurrentLoc.X,
                    this.CurrentLoc.Y,
                    this.CurrentLoc.Z
                ),
                CurrentWorldLoc = new Point2<int>
                (
                    this.CurrentWorldLoc.X,
                    this.CurrentWorldLoc.Y
                ),
                ModelCode = this.ModelCode
            };
        }
    }
}
