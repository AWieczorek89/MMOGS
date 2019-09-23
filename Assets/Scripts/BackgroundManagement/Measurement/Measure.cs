using BackgroundManagement.Measurement.Units;
using System;
using UnityEngine;

namespace BackgroundManagement.Measurement
{
    public static class Measure
    {
        public enum AngleType
        {
            Degrees,
            Radians
        }

        public static int GetTimeMsBetween(DateTime t1, DateTime t2)
        {
            TimeSpan span = t2 - t1;
            return Math.Abs((int)span.TotalMilliseconds);
        }

        /// <summary>
        /// Returns an angle between two 2D vectors
        /// </summary>
        /// <param name="vA">vector A</param>
        /// <param name="vB">vector B</param>
        /// <returns></returns>
        public static double GetAngleBetweenVectors(Point2<double> vA, Point2<double> vB, AngleType angleType = AngleType.Degrees)
        {
            double angle = Math.Atan2(vA.Y, vA.X) - Math.Atan2(vB.Y, vB.X);
            angle = angle * 360 / (2 * Math.PI);

            if (angle < 0)
                angle += 360;

            if (angleType == AngleType.Radians)
                angle = DegreesToRadians(angle);

            return angle;
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        public static float CalculateAverageTParamByMovementParameters(Point3<double> startingLoc, Point3<double> currentLoc, Point3<double> destinationLoc)
        {
            double tParam = 1;

            double xDistanceTotal = Math.Abs(destinationLoc.X - startingLoc.X);
            double yDistanceTotal = Math.Abs(destinationLoc.Y - startingLoc.Y);
            double zDistanceTotal = Math.Abs(destinationLoc.Z - startingLoc.Z);

            double xDistanceDone = Math.Abs(destinationLoc.X - currentLoc.X);
            double yDistanceDone = Math.Abs(destinationLoc.Y - currentLoc.Y);
            double zDistanceDone = Math.Abs(destinationLoc.Z - currentLoc.Z);

            double xTParam = (xDistanceTotal > 0f ? xDistanceDone / xDistanceTotal : 1);
            double yTParam = (yDistanceTotal > 0f ? yDistanceDone / yDistanceTotal : 1);
            double zTParam = (zDistanceTotal > 0f ? zDistanceDone / zDistanceTotal : 1);

            tParam = (xTParam + yTParam + zTParam) / 3;
            return Mathf.Clamp(Convert.ToSingle(tParam), 0f, 1f);
        }
        
        public static int CalculateTimeArrivalInMs(Vector3 pointFrom, Vector3 pointTo, float velocity)
        {
            float distance = Vector3.Distance //note that this method calculates distance only on XZ plane!
            (
                new Vector3 (pointFrom.x, 0f, pointFrom.z),
                new Vector3 (pointTo.x, 0f, pointTo.z)
            );

            return CalculateTimeArrivalInMs(distance, velocity);
        }

        public static int CalculateTimeArrivalInMs(float distance, float velocity)
        {
            if (velocity == 0f)
                return 0;

            float timeInSeconds = distance / velocity; //t = s / v
            return Convert.ToInt32(timeInSeconds * 1000);
        }

        /// <summary>
        /// Calculates moving angle on XZ plane
        /// </summary>
        public static float CalculateMovingAngle(Vector3 pointFrom, Vector3 pointTo)
        {
            Point2<double> direction = new Point2<double>
            (
                pointTo.x - pointFrom.x,
                pointTo.z - pointFrom.z
            );

            double angle = GetAngleBetweenVectors
            (
                new Point2<double>(1, 0), //X axis
                direction
            );

            return Convert.ToSingle(angle);
        }
    }
}
