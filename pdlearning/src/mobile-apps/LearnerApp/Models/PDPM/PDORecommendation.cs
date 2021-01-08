using System.Collections.Generic;

namespace LearnerApp.Models.PDPM
{
    public class PDORecommendation
    {
        public int TotalItems { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public bool HasMoreData { get; set; }

        public List<PDORecommendationItem> Items { get; set; }

        public AdditionalProperties AdditionalProperties { get; set; }
    }
}
