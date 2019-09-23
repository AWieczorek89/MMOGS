using MMOGS.Measurement.Units;
using MMOGS.Models.GameState;
using System;

namespace MMOGS.Measurement
{
    public static class Measure
    {
        public enum AngleType
        {
            Degrees,
            Radians
        }

        private static Random _random = new Random();
        
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

        public static double GetDistanceBetweenPoints(Point3<double> p1, Point3<double> p2)
        {
            return Math.Sqrt( Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2) + Math.Pow((p2.Z - p1.Z), 2) );
        }

        /// <summary>
        /// y = a*x + b
        /// </summary>
        public static void GetLineParameters(double x, double y, double angle, AngleType angleType, out double a, out double b)
        {
            a = Math.Tan
            (
                angleType == AngleType.Radians ?
                angle :
                DegreesToRadians(angle)
            );

            b = y - (a * x);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        public static double Lerp(double fromValue, double toValue, double t)
        {
            return (fromValue + ((toValue - fromValue) * t));
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }

        public static int GetRandomNumber(int minimum, int maximum)
        {
            return _random.Next(minimum, maximum + 1);
        }

        public static decimal CalculatePercent(decimal max, decimal current)
        {
            if (max == 0)
                return 0;

            decimal result = (current * 100) / max;
            return Clamp(0, 100, result);
        }

        public static double CalculatePercent(double max, double current)
        {
            if (max == 0)
                return 0;

            double result = (current * 100) / max;
            return Clamp(0, 100, result);
        }

        public static double Clamp(double min, double max, double current)
        {
            double result = current;
            result = (result > max ? max : result);
            result = (result < min ? min : result);
            return result;
        }

        public static decimal Clamp(decimal min, decimal max, decimal current)
        {
            decimal result = current;
            result = (result > max ? max : result);
            result = (result < min ? min : result);
            return result;
        }

        public static int Clamp(int min, int max, int current)
        {
            int result = current;
            result = (result > max ? max : result);
            result = (result < min ? min : result);
            return result;
        }

        public static double GetMax(params double[] values)
        {
            if (values.Length == 0)
                return 0;

            double result = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (i == 0)
                {
                    result = values[i];
                    continue;
                }

                if (values[i] > result)
                    result = values[i];
            }

            return result;
        }

        /// <summary>
        /// Gets a (t) parameter single iteration value used in Lerp() methods. 
        /// A result is determined by max difference on X, Y or Z coordinate (depending on which is the highest).
        /// </summary>
        public static double GetTParamStep(Point3<double> pointFrom, Point3<double> pointTo)
        {
            double tStep = 0;

            double movDiffX = Math.Abs(pointTo.X - pointFrom.X);
            double movDiffY = Math.Abs(pointTo.Y - pointFrom.Y);
            double movDiffZ = Math.Abs(pointTo.Z - pointFrom.Z);
            double divider = GetMax(movDiffX, movDiffY, movDiffZ) + 1;
            
            if (divider < 1.0000)
                divider = 1.0000;

            tStep = Math.Round((1.0000 / divider), 4);
            return tStep;
        }
    }
}
