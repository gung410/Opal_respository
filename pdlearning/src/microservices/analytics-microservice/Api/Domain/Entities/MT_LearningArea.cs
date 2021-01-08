using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_LearningArea
    {
        public MT_LearningArea()
        {
            CamCourseLearningArea = new HashSet<CAM_CourseLearningArea>();
            MtLearningSubArea = new HashSet<MT_LearningSubArea>();
        }

        public Guid LearningAreaId { get; set; }

        public Guid LearningDimensionId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual MT_LearningDimension LearningDimension { get; set; }

        public virtual ICollection<CAM_CourseLearningArea> CamCourseLearningArea { get; set; }

        public virtual ICollection<MT_LearningSubArea> MtLearningSubArea { get; set; }
    }
}
