namespace LocationMessanger.Controllers
{
    public static class ConvertHelper
    {
        public static double? ConvertDouble(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (double.TryParse(value, out double result))
                {
                    return result;
                }
            }
            return null;
        }
        public static int ConvertInt(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (int.TryParse(value, out int result))
                    return result;
            }
            return 0;
        }
    }
}
