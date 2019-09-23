using System;
using System.Collections.Generic;
using System.Drawing;

namespace ArgbConverter.ProcessingClasses
{
    public static class DataProcessor
    {
        public enum AngleType
        {
            Degrees,
            Radians
        }

        public enum BezierControlPoint
        {
            Off,
            On
        }

        private static Random random = new Random();

        #region Bezier

        public static List<Point> MultiBezierCurve
        (
            Point[] points,
            double tStep
        )
        {
            List<Point> resultPoints = new List<Point>();

            if (points.Length < 3)
                return resultPoints;

            Point p0 = new Point(0, 0);
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            double mid1X = 0;
            double mid1Y = 0;
            double mid2X = 0;
            double mid2Y = 0;
            Point midPoint1 = new Point(0, 0);
            Point midPoint2 = new Point(0, 0);

            #region Rysowanie pierwszej linii - prosta
            if (points.Length >= 2)
            {
                Point point1 = points[0];
                Point point2 = points[1];

                for (double t = 0; t <= 0.5; t += tStep)
                {
                    Point pFinal = new Point
                    (
                        Convert.ToInt32(Lerp(point1.X, point2.X, t)),
                        Convert.ToInt32(Lerp(point1.Y, point2.Y, t))
                    );
                    resultPoints.Add(pFinal);
                }
            }

            #endregion

            #region Tworzenie krzywych Beziera
            for (int i = 0; i < points.Length - 2; i++)
            {
                p0 = points[i];
                p1 = points[i + 1];
                p2 = points[i + 2];

                mid1X = (p0.X + p1.X) / 2;
                mid1Y = (p0.Y + p1.Y) / 2;
                mid2X = (p1.X + p2.X) / 2;
                mid2Y = (p1.Y + p2.Y) / 2;
                midPoint1.X = Convert.ToInt32(mid1X);
                midPoint1.Y = Convert.ToInt32(mid1Y);
                midPoint2.X = Convert.ToInt32(mid2X);
                midPoint2.Y = Convert.ToInt32(mid2Y);
                
                for (double t = 0; t <= 1; t += tStep)
                {
                    Point pFinal = new Point(0, 0);

                    QuadraticBezier
                    (
                        midPoint1,
                        p1,
                        midPoint2,
                        t,
                        ref pFinal
                    );

                    resultPoints.Add(pFinal);
                }
            }

            #endregion

            #region Rysowanie ostatniej linii - prosta
            if (points.Length >= 2)
            {
                Point point1 = points[points.Length - 2];
                Point point2 = points[points.Length - 1];

                for (double t = 0.5; t <= 1; t += tStep)
                {
                    Point pFinal = new Point
                    (
                        Convert.ToInt32(Lerp(point1.X, point2.X, t)),
                        Convert.ToInt32(Lerp(point1.Y, point2.Y, t))
                    );
                    resultPoints.Add(pFinal);
                }
            }

            #endregion

            return resultPoints;
        }
        
        public static void CubicBezier
        (
            Point p0,
            Point p1,
            Point p2,
            Point p3,
            double t,
            ref Point pFinal
        )
        {
            if (pFinal == null)
                pFinal = new Point(0, 0);

            double finalX =
                Math.Pow(1 - t, 3) * p0.X +
                Math.Pow(1 - t, 2) * 3 * t * p1.X +
                (1 - t) * 3 * t * t * p2.X +
                t * t * t * p3.X;
            double finalY =
                Math.Pow(1 - t, 3) * p0.Y +
                Math.Pow(1 - t, 2) * 3 * t * p1.Y +
                (1 - t) * 3 * t * t * p2.Y +
                t * t * t * p3.Y;

            pFinal.X = Convert.ToInt32(finalX);
            pFinal.Y = Convert.ToInt32(finalY);
        }

        public static void QuadraticBezierCp
        (
            Point p0,
            Point p1,
            Point p2,
            double t,
            ref Point pFinal
        )
        {
            Point controlPoint = new Point(); //punkt, przez który przechodzi krzywa
            controlPoint.X = p1.X * 2 - (p0.X + p2.X) / 2;
            controlPoint.Y = p1.Y * 2 - (p0.Y + p2.Y) / 2;

            QuadraticBezier (p0, controlPoint, p2, t, ref pFinal);
        }

        public static void QuadraticBezier
        (
            Point p0,
            Point p1,
            Point p2,
            double t,
            ref Point pFinal
        )
        {
            if (pFinal == null)
                pFinal = new Point(0, 0);
            
            double finalX =
                Math.Pow(1 - t, 2) * p0.X +
                (1 - t) * 2 * t * p1.X +
                t * t * p2.X;
            double finalY =
                Math.Pow(1 - t, 2) * p0.Y +
                (1 - t) * 2 * t * p1.Y +
                t * t * p2.Y;

            pFinal.X = Convert.ToInt32(finalX);
            pFinal.Y = Convert.ToInt32(finalY);
        }

        #endregion

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
            return random.NextDouble() * (maximum - minimum) + minimum;
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

        public static decimal Clamp (decimal min, decimal max, decimal current)
        {
            decimal result = current;
            result = (result > max ? max : result);
            result = (result < min ? min : result);
            return result;
        }

        public static int Clamp (int min, int max, int current)
        {
            int result = current;
            result = (result > max ? max : result);
            result = (result < min ? min : result);
            return result;
        }
    }
}
