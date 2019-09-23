namespace MMOGS.Models.Database
{
    public class DbCharactersData
    {
        public int CharId { get; private set; } = -1;
        public string Name { get; private set; } = "";
        public int AccId { get; private set; } = -1;
        public bool IsNpc { get; private set; } = false;
        public string NpcAltName { get; private set; } = "";
        public bool IsOnWorldMap { get; private set; } = false;
        public int WmId { get; private set; } = -1;
        public int TerrainParentId { get; private set; } = -1;
        public decimal LocalPosX { get; set; } = 0.0000M;
        public decimal LocalPosY { get; set; } = 0.0000M;
        public decimal LocalPosZ { get; set; } = 0.0000M;
        public decimal LocalAngle { get; set; } = 0.0000M;
        public string ModelCode { get; private set; } = "";
        public int HairstyleId { get; private set; } = 0;

        public DbCharactersData
        (
            int charId,
            string name, 
            int accId,
            bool isNpc,
            string npcAltName,
            bool isOnWorldMap,
            int wmId,
            int terrainParentId,
            decimal localPosX,
            decimal localPosY,
            decimal localPosZ,
            decimal localAngle,
            string modelCode,
            int hairstyleId
        )
        {
            this.CharId = charId;
            this.Name = name;
            this.AccId = accId;
            this.IsNpc = isNpc;
            this.NpcAltName = npcAltName;
            this.IsOnWorldMap = isOnWorldMap;
            this.WmId = wmId;
            this.TerrainParentId = terrainParentId;
            this.LocalPosX = localPosX;
            this.LocalPosY = localPosY;
            this.LocalPosZ = localPosZ;
            this.LocalAngle = localAngle;
            this.ModelCode = modelCode;
            this.HairstyleId = hairstyleId;
        }

        public DbCharactersData Copy()
        {
            return new DbCharactersData
            (
                this.CharId,
                this.Name,
                this.AccId,
                this.IsNpc,
                this.NpcAltName,
                this.IsOnWorldMap,
                this.WmId,
                this.TerrainParentId,
                this.LocalPosX,
                this.LocalPosY,
                this.LocalPosZ,
                this.LocalAngle,
                this.ModelCode,
                this.HairstyleId
            );
        }
    }
}
