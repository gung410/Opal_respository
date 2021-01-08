using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Client
{
    public class CustomPaginatedList<T> : PaginatedList<T> where T : class
    {
        public CustomPaginatedList()
        {
        }

        public CustomPaginatedList(List<T> items, int pageIndex, int pageSize, bool hasMoreData) :
            base(items, pageIndex, pageSize, hasMoreData)
        {
        }
        public CustomPaginatedList(List<T> items, int pageIndex, int pageSize, bool hasMoreData, int? totalItemCount) :
           base(items, pageIndex, pageSize, hasMoreData)
        {
            this.TotalItemCount = totalItemCount;
        }
        public int? TotalItemCount { get; set; }
    }
}