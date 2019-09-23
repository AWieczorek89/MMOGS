using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgbConverter.DataModels
{
    public class AreaGroupingInfo
    {
        public int IdFrom { get; private set; } = -1;
        public int IdTo { get; private set; } = -1;

        private static List<AreaGroupingInfo> IdAssignmentList = new List<AreaGroupingInfo>();

        public static void Clear()
        {
            IdAssignmentList.Clear();
        }

        /// <summary>
        /// Dodawanie informacji n/t przypisywania nowego ID strefy do obecnego (zbiorcze) - z uwzględnieniem przypisywania jak najniższego ID docelowego (idTo)
        /// </summary>
        public static void AddAssignment(List<int> idFromList, int idTo)
        {
            foreach (int idFrom in idFromList)
            {
                AddAssignment(idFrom, idTo);
            }
        }

        /// <summary>
        /// Dodawanie informacji n/t przypisywania nowego ID strefy do obecnego - z uwzględnieniem przypisywania jak najniższego ID docelowego (idTo)
        /// </summary>
        public static void AddAssignment(int idFrom, int idTo)
        {
            bool exists = false;

            foreach (AreaGroupingInfo info in IdAssignmentList)
            {
                if (info.IdFrom == idFrom)
                {
                    exists = true;

                    if (idTo < info.IdTo)
                        info.IdTo = idTo;

                    break;
                }
            }

            if (!exists)
            {
                AreaGroupingInfo info = new AreaGroupingInfo();
                info.IdFrom = idFrom;
                info.IdTo = idTo;
                IdAssignmentList.Add(info);
            }
        }

        /// <summary>
        /// Metoda analizująca połączenia między strefami i unifikująca do jednej docelowej
        /// </summary>
        public static void AnalyzeAreaRelationships()
        {
            AreaGroupingInfo info = null;

            for (int i = 0; i < IdAssignmentList.Count; i++)
            {
                for (int j = 0; j < IdAssignmentList.Count; j++)
                {
                    info = IdAssignmentList[j];
                    
                    foreach (AreaGroupingInfo infoSearch in IdAssignmentList)
                    {
                        if (infoSearch.IdFrom == info.IdTo)
                        {
                            info.IdTo = infoSearch.IdTo;
                            break;
                        }
                    }
                }
            }

            info = null;
        }

        public static List<AreaGroupingInfo> GetGroupingInfo()
        {
            return IdAssignmentList;
        }
    }
}
