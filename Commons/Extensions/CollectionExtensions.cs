using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Extensions
{
    public static class CollectionExtensions
    {
        public static void Remove<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var itemsToRemove = collection.Where(predicate).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                collection.Remove(itemToRemove);
            }
        }
    }
}
