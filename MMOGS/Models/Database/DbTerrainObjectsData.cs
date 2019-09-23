namespace MMOGS.Models.Database
{
    public class DbTerrainObjectsData
    {
        public int ToId { get; private set; } = -1;
        public int WmId { get; private set; } = -1;
        public int LocalPosX { get; private set; } = -1;
        public int LocalPosY { get; private set; } = -1;
        public int LocalPosZ { get; private set; } = -1;
        public int TodId { get; private set; } = -1;
        public int ParentId { get; private set; } = -1;
        public bool IsParentalTeleport { get; private set; } = false;
        public bool IsExit { get; private set; } = false;

        public DbTerrainObjectsData
        (
            int toId,
            int wmId,
            int localPosX,
            int localPosY,
            int localPosZ,
            int todId,
            int parentId,
            bool isParentalTeleport,
            bool isExit
        )
        {
            this.ToId = toId;
            this.WmId = wmId;
            this.LocalPosX = localPosX;
            this.LocalPosY = localPosY;
            this.LocalPosZ = localPosZ;
            this.TodId = todId;
            this.ParentId = parentId;
            this.IsParentalTeleport = isParentalTeleport;
            this.IsExit = isExit;
        }
    }
}
