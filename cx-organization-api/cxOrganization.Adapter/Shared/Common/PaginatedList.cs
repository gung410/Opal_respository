using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Shared.Common
{
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T> where T : class
    {
        private const int MaximumRecordsReturn = 100000;

        /// <summary>The index of the currently displayed page</summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// The number of records to display on a single page. The default is MaximumRecordsReturn: 10000
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>Still have more data to load</summary>
        public bool HasMoreData { get; private set; }

        /// <summary>Gets a collection of objects that represent the data</summary>
        public List<T> Items { get; private set; }

        /// <summary>constructor</summary>
        public PaginatedList()
        {
            this.Items = new List<T>();
            this.PageIndex = 0;
            this.PageSize = 0;
            this.HasMoreData = false;
        }

        /// <summary>Constructor</summary>
        /// <param name="items"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="hasMoreData"></param>
        public PaginatedList(List<T> items, int pageIndex, int pageSize, bool hasMoreData)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (pageIndex == 0)
                pageIndex = 1;
            if (pageSize == 0 || pageSize > 100000)
                pageSize = 100000;
            this.Items = items;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.HasMoreData = hasMoreData;
        }
    }
}

