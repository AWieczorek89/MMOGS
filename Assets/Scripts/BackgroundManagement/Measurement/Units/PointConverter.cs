using System;
using UnityEngine;

namespace BackgroundManagement.Measurement.Units
{
    public static class PointConverter
    {
        public static Vector3 Point3ToVector(Point3<double> point)
        {
            return new Vector3(Convert.ToSingle(point.X), Convert.ToSingle(point.Y), Convert.ToSingle(point.Z));
        }

        public static Vector3 Point3ToVector(Point3<int> point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }

        public static Vector3 Point3ToVector(Point3<float> point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }

        public static Vector3 Point3ToVector(Point3<decimal> point)
        {
            return new Vector3(Convert.ToSingle(point.X), Convert.ToSingle(point.Y), Convert.ToSingle(point.Z));
        }
        
        public static Vector3 Point2ToVector(Point2<int> point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static Vector3 Point2ToVector(Point2<float> point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static Vector3 Point2ToVector(Point2<decimal> point)
        {
            return new Vector2(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
        }
    }
}
