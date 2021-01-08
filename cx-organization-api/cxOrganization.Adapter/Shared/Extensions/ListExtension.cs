using System.Collections.Generic;

namespace cxOrganization.Adapter.Shared.Extensions
{
    public static class ListExtension
    {
        public static bool IsNotNullOrEmpty<T>(this List<T> filterList)
        {
            return filterList != null && filterList.Count > 0;
        }
        public static bool IsNullOrEmpty<T>(this List<T> filterList)
        {
            return filterList == null || filterList.Count == 0;
        }
    }
}
