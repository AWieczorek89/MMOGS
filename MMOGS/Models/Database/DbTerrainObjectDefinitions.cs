namespace MMOGS.Models.Database
{
    public class DbTerrainObjectDefinitions
    {
        public static int MaxCollisionXOfAllObjects { get; private set; } = 0;
        public static int MaxCollisionYOfAllObjects { get; private set; } = 0;
        public static int MaxCollisionZOfAllObjects { get; private set; } = 0;

        public int TodId { get; private set; } = -1;
        public string Code { get; private set; } = "";
        public int CollisionX { get; private set; } = 0;
        public int CollisionY { get; private set; } = 0;
        public int CollisionZ { get; private set; } = 0;
        public bool IsTerrain { get; private set; } = false;
        public bool IsPlatform { get; private set; } = false;
        public bool IsObstacle { get; private set; } = false;

        public DbTerrainObjectDefinitions
        (
            int todId,
            string code,
            int collisionX,
            int collisionY,
            int collisionZ,
            bool isTerrain,
            bool isPlatform,
            bool isObstacle
        )
        {
            this.TodId = todId;
            this.Code = code;
            this.CollisionX = collisionX;
            this.CollisionY = collisionY;
            this.CollisionZ = collisionZ;
            this.IsTerrain = isTerrain;
            this.IsPlatform = isPlatform;
            this.IsObstacle = isObstacle;

            if (collisionX > DbTerrainObjectDefinitions.MaxCollisionXOfAllObjects)
                DbTerrainObjectDefinitions.MaxCollisionXOfAllObjects = collisionX;

            if (collisionY > DbTerrainObjectDefinitions.MaxCollisionYOfAllObjects)
                DbTerrainObjectDefinitions.MaxCollisionYOfAllObjects = collisionY;

            if (collisionZ > DbTerrainObjectDefinitions.MaxCollisionZOfAllObjects)
                DbTerrainObjectDefinitions.MaxCollisionZOfAllObjects = collisionZ;
        }
    }
}
