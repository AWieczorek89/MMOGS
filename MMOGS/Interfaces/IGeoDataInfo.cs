using MMOGS.Measurement.Units;
using MMOGS.Models;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.GameState;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMOGS.Interfaces
{
    public interface IGeoDataInfo
    {
        Task<BoxedData> GetLocalGeoDataTaskStart(int wmId, int parentObjectId, Point3<int> from, Point3<int> to);
        Task<BoxedData> GetLocalGeoDataOfExitElementsTaskStart(int wmId, int parentObjectId);

        BoxedData GetLocalGeoData(int wmId, int parentObjectId, Point3<int> from, Point3<int> to);
        BoxedData GetLocalGeoDataOfExitElements(int wmId, int parentObjectId);

        List<WorldPlaceDataDetails> GetWorldPlaceDataDetails();
        WorldPlaceDataDetails GetWorldPlaceDataDetails(int coordX, int coordY);
        Point2<int> GetWorldCoordsByWmId(int wmId);
        WorldPlaceData GetWorldPlaceByWmId(int wmId);
    }
}
