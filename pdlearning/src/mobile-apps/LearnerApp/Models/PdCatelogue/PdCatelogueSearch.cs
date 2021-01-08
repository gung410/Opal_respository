using System.Collections.Generic;

namespace LearnerApp.Models.PdCatelogue
{
    public class PdCatelogueSearch
    {
        public int Total { get; set; }

        public List<PdResourceStatistic> ResourceStatistics { get; set; }

        public List<PdCatelogueResource> Resources { get; set; }
    }
}
