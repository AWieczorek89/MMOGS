namespace MMOGS.Models
{
    public class TerrainConstraintsSettings
    {
        public int ConstraintX { get; private set; } = 10;
        public int ConstraintY { get; private set; } = 10;
        public int ConstraintZ { get; private set; } = 10;

        public TerrainConstraintsSettings(int constraintX, int constraintY, int constraintZ)
        {
            this.ConstraintX = constraintX;
            this.ConstraintY = constraintY;
            this.ConstraintZ = constraintZ;
        }
    }
}
