using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Extensions
{
    public static class EnumExtensions
    {
        public static IDictionary<int, string> AsDictionary<TEnum>() where TEnum : struct
        {
            var enumType = typeof(TEnum);

            return Enum.GetValues(enumType)
                       .Cast<TEnum>()
                       .ToDictionary(x => Convert.ToInt32(x), x => Enum.GetName(enumType, x));
        }
    }
}
