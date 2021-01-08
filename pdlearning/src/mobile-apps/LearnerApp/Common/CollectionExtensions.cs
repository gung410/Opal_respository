using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LearnerApp.Controls.LearnerObservableCollection;

namespace LearnerApp.Common
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ObservableCollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// Use <see cref="LearnerObservableCollection{T}.AddRange"/> instead.
        /// </summary>
        /// <param name="collection">Current collection.</param>
        /// <param name="items">New Items.</param>
        /// <typeparam name="T">Generic type of item.</typeparam>
        public static void AddRange<T>(this ObservableCollection<T> collection, List<T> items)
        {
            if (!items.IsNullOrEmpty() && collection != null)
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
        }

        public static bool IsNullOrEmpty<T>(this List<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
