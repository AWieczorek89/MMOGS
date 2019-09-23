using System;

namespace MMOGS.Measurement.Units
{
    public static class PointConverter
    {
        public static Point3<int> Point3DoubleToInt(Point3<double> point)
        {
            return new Point3<int>
            (
                Convert.ToInt32(Math.Round(point.X)),
                Convert.ToInt32(Math.Round(point.Y)),
                Convert.ToInt32(Math.Round(point.Z))
            );
        }
    }
}
