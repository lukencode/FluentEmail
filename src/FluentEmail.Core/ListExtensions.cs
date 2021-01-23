using System;
using System.Collections.Generic;

namespace FluentEmail.Core
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> consumer)
        {
            foreach (T item in enumerable)
            {
                consumer(item);
            }
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list is List<T> listT)
            {
                listT.AddRange(items);
            }
            else
            {
                foreach (T item in items)
                {
                    list.Add(item);
                }
            }
        }
    }
}
