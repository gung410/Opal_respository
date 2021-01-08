using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Business.Extensions
{
    public static class CollectionExtention
    {
        public static bool IsNullOrEmpty(this ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static List<int> ToInts(this IEnumerable<long> values)
        {
            if (values == null) return null;
            return values.Select(v => (int) v).ToList();
        }
    }
}
