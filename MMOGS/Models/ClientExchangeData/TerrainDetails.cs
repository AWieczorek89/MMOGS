using MMOGS.Measurement.Units;

namespace MMOGS.Models.ClientExchangeData
{
    public class TerrainDetails
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
    }
}
