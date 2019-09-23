using MMOGS.Measurement.Units;

namespace MMOGS.Models.GameState
{
    public class GeoDataElement
    {
        public enum Type
        {
            None,
            Terrain,
            Platform,
            Obstacle
        }

        public Point3<int> Location { get; set; }
        public Point3<int> ColliderSize { get; set; }
        public Type ElementType { get; set; }
        public bool IsExit { get; set; } = false;

        public GeoDataElement(Point3<int> location, Point3<int> colliderSize, Type elementType, bool isExit)
        {
            this.Location = location;
            this.ColliderSize = colliderSize;
            this.ElementType = elementType;
            this.IsExit = isExit;
        }

        public GeoDataElement(int locationX, int locationY, int locationZ, int colliderSizeX, int colliderSizeY, int colliderSizeZ)
        {
            this.Location = new Point3<int>(locationX, locationY, locationZ);
            this.ColliderSize = new Point3<int>(colliderSizeX, colliderSizeY, colliderSizeZ);
        }
    }
}
