using MMOGS.Measurement.Units;
using System;

namespace MMOGS.Models.GameState
{
    public class CharacterMovementDetails : IDisposable, ICloneable
    {
        public enum MovementType
        {
            World,
            Local,
            SwitchMovementType
        }

        public MovementType Type { get; private set; } = MovementType.World;
        public int CharId { get; private set; } = -1;
        public Point3<double> OldLocationLocal { get; private set; } = new Point3<double>(0, 0, 0);
        public Point3<double> NewLocationLocal { get; private set; } = new Point3<double>(0, 0, 0);
        public int TimeArrivalMsLocal { get; private set; } = 0;
        public Point2<int> OldLocationWorld { get; private set; } = new Point2<int>(0, 0);
        public Point2<int> NewLocationWorld { get; private set; } = new Point2<int>(0, 0);

        public CharacterMovementDetails
        (
            MovementType type,
            int charId,
            Point3<double> oldLocationLocal,
            Point3<double> newLocationLocal,
            int timeArrivalMsLocal,
            Point2<int> oldLocationWorld,
            Point2<int> newLocationWorld
        )
        {
            this.Type = type;
            this.CharId = charId;
            this.OldLocationLocal = oldLocationLocal;
            this.NewLocationLocal = newLocationLocal;
            this.TimeArrivalMsLocal = timeArrivalMsLocal;
            this.OldLocationWorld = oldLocationWorld;
            this.NewLocationWorld = newLocationWorld;
        }

        public void Dispose()
        {
            this.OldLocationLocal = null;
            this.NewLocationLocal = null;
            this.OldLocationWorld = null;
            this.NewLocationWorld = null;
        }

        public object Clone()
        {
            return new CharacterMovementDetails
            (
                this.Type,
                this.CharId,
                new Point3<double>(this.OldLocationLocal.X, this.OldLocationLocal.Y, this.OldLocationLocal.Z),
                new Point3<double>(this.NewLocationLocal.X, this.NewLocationLocal.Y, this.NewLocationLocal.Z),
                this.TimeArrivalMsLocal,
                new Point2<int>(this.OldLocationWorld.X, this.OldLocationWorld.Y),
                new Point2<int>(this.NewLocationWorld.X, this.NewLocationWorld.Y)
            );
        }
    }
}
