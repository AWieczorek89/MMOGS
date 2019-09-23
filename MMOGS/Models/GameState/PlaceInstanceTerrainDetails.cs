using MMOGS.Models.Database;
using System;

namespace MMOGS.Models.GameState
{
    public class PlaceInstanceTerrainDetails : IDisposable
    {
        public int ToId { get; private set; } = -1;
        public int WmId { get; private set; } = -1;
        public int LocalPosX { get; private set; } = -1;
        public int LocalPosY { get; private set; } = -1;
        public int LocalPosZ { get; private set; } = -1;
        public int ParentId { get; private set; } = -1;
        public bool IsParentalTeleport { get; private set; } = false;
        public bool IsExit { get; private set; } = false;
        public DbTerrainObjectDefinitions TerrainDefinition { get; private set; } = null;

        public PlaceInstanceTerrainDetails
        (
            int toId,
            int wmId,
            int localPosX,
            int localPosY,
            int localPosZ,
            int parentId,
            bool isParentalTeleport,
            bool isExit,
            DbTerrainObjectDefinitions terrainDefinition
        )
        {
            this.ToId = toId;
            this.WmId = wmId;
            this.LocalPosX = localPosX;
            this.LocalPosY = localPosY;
            this.LocalPosZ = localPosZ;
            this.ParentId = parentId;
            this.IsParentalTeleport = isParentalTeleport;
            this.IsExit = isExit;
            this.TerrainDefinition = terrainDefinition;
        }

        public PlaceInstanceTerrainDetails Copy()
        {
            return new PlaceInstanceTerrainDetails
            (
                this.ToId,
                this.WmId,
                this.LocalPosX,
                this.LocalPosY,
                this.LocalPosZ,
                this.ParentId,
                this.IsParentalTeleport,
                this.IsExit,
                this.TerrainDefinition
            );
        }

        public void Dispose()
        {
            this.TerrainDefinition = null;
        }
    }
}
