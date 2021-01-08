using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microservice.Form.Common.Extensions
{
    internal static class EnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool ContainsAll<T>(this IEnumerable<T> list1, params T[] list2)
        {
            return list1.Intersect(list2).Count() == list2.Count();
        }

        public static IEnumerable<IEnumerable<T>> DuplicatedItemGroups<T, TKey>(this IEnumerable<T> items, Func<T, TKey> duplicatedByProperty)
        {
            return items.GroupBy(duplicatedByProperty).Where(p => p.Count() > 1).Select(p => p.ToList());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var result = list.ToList();
            var rng = new Random();
            int n = result.Count();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = result[k];
                result[k] = result[n];
                result[n] = value;
            }

            return result;
        }

        public static IEnumerable<TItem> Upsert<TItem, TUpdateBy>(
            this IEnumerable<TItem> sources,
            IEnumerable<TItem> values,
            Expression<Func<TItem, TUpdateBy>> updateBy,
            Action<TItem, TItem> customUpdateAction = null)
        {
            var sourcesDictionary = sources.ToDictionary(updateBy.Compile());
            var valuesDictionary = values.ToDictionary(updateBy.Compile());
            foreach (var valuesDictionaryKeyValuePair in valuesDictionary)
            {
                if (sourcesDictionary.ContainsKey(valuesDictionaryKeyValuePair.Key))
                {
                    if (customUpdateAction != null)
                    {
                        customUpdateAction(sourcesDictionary[valuesDictionaryKeyValuePair.Key], valuesDictionaryKeyValuePair.Value);
                    }
                    else
                    {
                        sourcesDictionary[valuesDictionaryKeyValuePair.Key] = valuesDictionaryKeyValuePair.Value;
                    }
                }
                else
                {
                    sourcesDictionary.Add(valuesDictionaryKeyValuePair.Key, valuesDictionaryKeyValuePair.Value);
                }
            }

            return sourcesDictionary.Select(p => p.Value).ToList();
        }
    }
}
