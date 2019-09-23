using BackgroundManagement.Measurement.Units;
using System;

namespace BackgroundManagement.Models.ClientExchangeData
{
    public class TerrainDetails : ICloneable
    {
        public int ToId { get; set; }
        public Point3<int> LocalPos { get; set; }
        public int ParentId { get; set; }
        public bool IsParentalTeleport { get; set; }
        public bool IsExit { get; set; }

        //terrain object definition
        public int TodId { get; set; }
        public string TodCode { get; set; }
        public Point3<int> TodCollision { get; set; }
        public bool TodIsTerrain { get; set; }
        public bool TodIsPlatform { get; set; }
        public bool TodIsObstacle { get; set; }

        public object Clone()
        {
            return new TerrainDetails
            {
                ToId = this.ToId,
                LocalPos = new Point3<int>
                (
                    this.LocalPos.X,
                    this.LocalPos.Y,
                    this.LocalPos.Z
                ),
                ParentId = this.ParentId,
                IsParentalTeleport = this.IsParentalTeleport,
                IsExit = this.IsExit,

                TodId = this.TodId,
                TodCode = this.TodCode,
                TodCollision = new Point3<int>
                (
                    this.TodCollision.X,
                    this.TodCollision.Y,
                    this.TodCollision.Z
                ),
                TodIsTerrain = this.TodIsTerrain,
                TodIsPlatform = this.TodIsPlatform,
                TodIsObstacle = this.TodIsObstacle
            };
        }
    }
}
