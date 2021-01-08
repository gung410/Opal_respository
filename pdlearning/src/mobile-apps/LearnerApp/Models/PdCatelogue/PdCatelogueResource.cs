using System;

namespace LearnerApp.Models.PdCatelogue
{
    public class PdCatelogueResource
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Publisher { get; set; }

        public string ResourceType { get; set; }

        public DateTime PublishDate { get; set; }
    }
}
