using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Albion_RMT_Empire_Tool_Beta
{
    static class CultureClass
    {
        public static CultureInfo GetCultureInfo(CultureInfo info)
        {
            info.NumberFormat.NumberDecimalSeparator = ",";
            info.NumberFormat.NumberGroupSeparator = "";
            return info;
        }
    }
}
