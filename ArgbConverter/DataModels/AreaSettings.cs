using System.Collections.Generic;
using System.Drawing;

namespace ArgbConverter.DataModels
{
    public class AreaSettings
    {
        public enum HairDrawingDirection
        {
            TopToBottom,
            LeftToRight,
            RightToLeft,
            BottomToTop,
            Radial,
            LinearX,
            LinearY
        }

        public enum HairCuttingSide
        {
            None,
            Beginning,
            End,
            BothSides
        }

        public enum BezierType
        {
            Quadratic,
            QuadraticCp,
            Cubic,
            Multi
        }

        public int AreaId { get; private set; } = -1;

        public Point3 BackgroundRgbPalette { get; set; } = new Point3(0, 0, 0);
        public Point3 ForegroundRgbPalette { get; set; } = new Point3(255, 255, 255);

        public HairDrawingDirection Direction { get; set; } = HairDrawingDirection.TopToBottom;
        public Point HairDirectionOffset { get; set; } = new Point(0, 0);
        public HairCuttingSide CuttingSide { get; set; } = HairCuttingSide.None;
        public decimal CuttingRangePercent { get; set; } = 10.0M;

        public BezierType BezierLineType { get; set; } = BezierType.QuadraticCp;
        public int MultiBezierPointCount { get; set; } = 4;
        public decimal BezierRandomRangePercent { get; set; } = 10.0M;
        public decimal BezierMarginPercent { get; set; } = 0.00M;

        public bool HairMapDefinesOpacity { get; set; } = false;
        public bool HairMapDefinesBrightness { get; set; } = false;
        public int DrawingIterations { get; set; } = 100;
        public int DrawingSteps { get; set; } = 5;
        public decimal DrawingStepIterationLossPercent { get; set; } = 10.00M;
        
        public AreaSettings(int areaId)
        {
            this.AreaId = areaId;
        }
    }
}
