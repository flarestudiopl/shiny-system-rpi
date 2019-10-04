using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        public static bool ContainsDigitsOnly(this string value) => value.All(x => x > 47 && x < 58);

        public static bool HasLengthBetween(this string value, int from, int to) => value.Length >= from && value.Length <= to;

        public static string CalculateHash(this string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));

                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
