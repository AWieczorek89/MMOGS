using System.Collections.Generic;

namespace MMOGS.Interfaces
{
    public interface ILogger
    {
        void UpdateLog(string logTxt);
        void UpdateLog(List<string> logTxtList);
        void ClearLog();
    }
}
