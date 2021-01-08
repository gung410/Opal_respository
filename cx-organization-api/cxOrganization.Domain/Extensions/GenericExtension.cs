using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Domain.Extensions
{
    public static class GenericExtension
    {
        public static bool hasDuplicateData<T>(this T[] array)
        {
            return array.GroupBy(item => item)
                         .Where(group => group.Count() > 1)
                         .Count() > 0;
        }
    }
}
