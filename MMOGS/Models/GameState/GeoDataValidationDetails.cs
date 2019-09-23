using MMOGS.Measurement.Units;
using System;

namespace MMOGS.Models.GameState
{
    public class GeoDataValidationDetails : IDisposable
    {
        public bool Valid { get; set; } = true;
        public Point3<double> LastValidMovementPoint { get; set; } = new Point3<double>(0, 0, 0);
        public double LastTParamValue { get; set; } = 0;

        public void Dispose()
        {
            this.LastValidMovementPoint = null;
        }
    }
}
