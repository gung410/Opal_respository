using System.Collections.Generic;

namespace LearnerApp.Models.PDPM
{
    public class PDOSInfoResponse
    {
        public int TotalItems { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public bool HasMoreData { get; set; }

        public IList<PDOResponse> Items { get; set; }
    }
}