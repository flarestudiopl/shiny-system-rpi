using System.Collections.Generic;

namespace Commons.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key)
                       ? dictionary[key]
                       : default(TValue);
        }
    }
}
