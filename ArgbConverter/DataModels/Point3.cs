namespace ArgbConverter.DataModels
{
    public struct Point3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
