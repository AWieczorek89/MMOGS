using MMOGS.Measurement.Units;
using System.Drawing;

namespace MMOGS.Interfaces
{
    public interface ICharacterActionManager
    {
        void MoveCharacterLocal(int charId, Point3<double> oldLocation, Point3<double> newLocation, int timeArrivalMs);
        void MoveCharacterWorld(int charId, Point2<int> oldLocation, Point2<int> newLocation);
        void SwitchCharacterMovementType (int charId);
    }
}
