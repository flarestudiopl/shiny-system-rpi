using System.Collections.Generic;

namespace Commons.Extensions
{
    public static class StringExtensions
    {
        public static string FormatWith(this string stringToFormat, params object[] values)
        {
            return values == null
                       ? stringToFormat
                       : string.Format(stringToFormat, values);
        }

        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        public static string JoinWith(this IEnumerable<string> values, string separator) => string.Join(separator, values);
    }
}
