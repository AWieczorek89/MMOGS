using System.Windows.Forms;

namespace ArgbConverter.DataModels
{
    public class HairGenUiReferenceContainer
    {
        public Panel MainAreaPanel { get; set; } = null;

        public NumericUpDown HairDirOffsetXNumericUpDown { get; set; } = null;
        public NumericUpDown HairDirOffsetYNumericUpDown { get; set; } = null;
        public NumericUpDown HairCuttingPercentNumericUpDown { get; set; } = null;
        public NumericUpDown BezierPointsCountNumericUpDown { get; set; } = null;
    }
}
