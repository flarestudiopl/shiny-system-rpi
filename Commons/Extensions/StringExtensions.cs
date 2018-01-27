namespace Commons.Extensions
{
    public static class StringExtensions
    {
        public static string FormatWith(this string stringToFormat, params object[] values)
        {
            return string.Format(stringToFormat, values);
        }
    }
}
