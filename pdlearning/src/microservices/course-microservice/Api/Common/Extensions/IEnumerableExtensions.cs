using System;
using System.Collections.Generic;
using System.Linq;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool ContainsAll<T>(this IEnumerable<T> list1, params T[] list2)
        {
            return list1.Intersect(list2).Count() == list2.Length;
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.Where(predicate).Distinct().ToList();
        }

        public static List<TResult> SelectFilterList<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            Func<TSource, TResult> selector)
        {
            return source.Where(predicate).SelectList(selector);
        }

        public static IEnumerable<T> ReverseList<T>(this IEnumerable<T> list)
        {
            IEnumerable<T> result = new List<T>();
            foreach (var item in list)
            {
                result = result.Prepend(item);
            }

            return result.ToList();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            return list ?? new List<T>();
        }

        /// <summary>
        /// Example: List[string]: ["a", "b"] ==Bind==> fn(value:string) => List[string] [value + "1", value + "2"] ==Return==> List[string] ["a1", "a2", "b1", "b2"].
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <typeparam name="TR">The item return type.</typeparam>
        /// <param name="list">The bind from list.</param>
        /// <param name="func">The bind func.</param>
        /// <returns>The bind list result.</returns>
        public static List<TR> Bind<T, TR>(this IEnumerable<T> list, Func<T, IEnumerable<TR>> func)
            => list.SelectMany(func).ToList();

        public static bool Empty<T>(this IEnumerable<T> list)
        {
            return !list.Any();
        }
    }
}
