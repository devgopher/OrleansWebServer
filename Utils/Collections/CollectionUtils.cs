using System;
using System.Collections.Generic;

namespace Utils.Collections
{
    public static class CollectionUtils
    {
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (T t in list)
                action(t);
        }
    }
}
