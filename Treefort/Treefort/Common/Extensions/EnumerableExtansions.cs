        using System;
using System.Collections.Generic;

namespace Treefort.Common.Extensions
{
    public static class EnumerableExtansions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (T item in list)
            {
                action(item);
            }
        }
    }
}
