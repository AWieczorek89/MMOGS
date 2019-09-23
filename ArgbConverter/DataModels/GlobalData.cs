using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgbConverter.DataModels
{
    public static class GlobalData
    {
        public static string[] ImageFormats { get; private set; } = new string[]
        {
            "png",
            "jpg",
            "jpeg",
            "bmp"
        };

        public static string ResultFileDirectory { get; private set; } = @".\result";
    }
}
