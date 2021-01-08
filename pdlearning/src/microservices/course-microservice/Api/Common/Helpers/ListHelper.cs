using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Course.Common.Helpers
{
    public static class ListHelper
    {
        public static HashSet<T> RandomPick<T>(List<T> items, int pickAmount, params T[] ignoreItems)
        {
            if (ignoreItems != null && ignoreItems.Length + pickAmount > items.Count)
            {
                throw new ArgumentException("IgnoreItems length + pickAmount must be less than or equal items count.");
            }

            var randomizedOtherItems = new HashSet<T>();
            var random = new Random();
            var distinctIgnoreItems = ignoreItems?.Distinct().ToHashSet();
            var filteredItems = distinctIgnoreItems != null ? items.Where(p => !distinctIgnoreItems.Contains(p)).ToList() : items;

            while (randomizedOtherItems.Count < pickAmount)
            {
                var randomizedPickedItem = filteredItems[random.Next(0, filteredItems.Count - 1)];
                randomizedOtherItems.Add(randomizedPickedItem);
                filteredItems.Remove(randomizedPickedItem);
            }

            return randomizedOtherItems;
        }
    }
}
