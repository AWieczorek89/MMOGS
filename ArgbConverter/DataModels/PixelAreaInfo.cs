using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgbConverter.DataModels
{
    public class PixelAreaInfo
    {
        public int AreaId { get; set; } = -1;
        public bool IsDeadZone { get; set; } = true;

        public PixelAreaInfo()
        {
        }

        public PixelAreaInfo(int areaId, bool isDeadZone)
        {
            this.AreaId = areaId;
            this.IsDeadZone = isDeadZone;
        }
    }
}
