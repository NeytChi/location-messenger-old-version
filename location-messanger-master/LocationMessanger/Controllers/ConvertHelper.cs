using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationMessanger.Controllers
{
    public static class ConvertHelper
    {
        public static double? ConvertDouble(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (Double.TryParse(value, out double result))
                    return result;
            }
            return null;
        }
        public static int ConvertInt(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (Int32.TryParse(value, out int result))
                    return result;
            }
            return 0;
        }
    }
}
