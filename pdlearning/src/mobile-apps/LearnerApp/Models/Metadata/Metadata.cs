using System.Collections.Generic;

namespace LearnerApp.Models
{
    public class Metadata
    {
        public Resource Resource { get; set; }

        public List<MetadataTag> MetadataTags { get; set; }
    }
}
