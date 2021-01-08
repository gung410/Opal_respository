using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microservice.Course.Common.Extensions
{
    public static class ICollectionExtensions
    {
        public static void RemoveAll<T>(this ICollection<T> collection, Expression<Func<T, bool>> predicate)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (predicate.Compile()(collection.ElementAt(i)))
                {
                    collection.Remove(collection.ElementAt(i));
                    i--;
                }
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, ICollection<T> item)
        {
            for (int i = 0; i < item.Count; i++)
            {
                collection.Add(item.ElementAt(i));
            }
        }

        public static void Update<TItem, TUpdateBy>(
            this ICollection<TItem> sources,
            IEnumerable<TItem> targets,
            Func<TItem, TUpdateBy> compareBy,
            Action<TItem, TItem> updateAction,
            Action<TItem> insertAction,
            Action<TItem> removeAction = null)
        {
            var sourceDictionary = sources.ToDictionary(compareBy);
            var targetDictionary = targets.ToDictionary(compareBy);

            foreach (var sourceDictionaryItem in sourceDictionary)
            {
                if (!targetDictionary.ContainsKey(sourceDictionaryItem.Key) && removeAction != null)
                {
                    removeAction(sourceDictionaryItem.Value);
                }
            }

            foreach (var targetDictionaryItem in targetDictionary)
            {
                if (sourceDictionary.ContainsKey(targetDictionaryItem.Key))
                {
                    updateAction(sourceDictionary[targetDictionaryItem.Key], targetDictionaryItem.Value);
                }
                else
                {
                    insertAction(targetDictionaryItem.Value);
                }
            }
        }
    }
}
