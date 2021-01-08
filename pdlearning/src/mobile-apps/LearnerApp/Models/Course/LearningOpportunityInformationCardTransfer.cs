using System.Collections.Generic;

namespace LearnerApp.Models.Learner
{
    public class LearningOpportunityInformationCardTransfer
    {
        public List<MetadataTag> MetadataTags { get; set; }

        public CourseExtendedInformation CourseExtendedInformation { get; set; }
    }
}
