using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_LearningSubArea
    {
        public MT_LearningSubArea()
        {
            CamCourseLearningSubArea = new HashSet<CAM_CourseLearningSubArea>();
        }

        public Guid LearningSubAreaId { get; set; }

        public Guid LearningAreaId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual MT_LearningArea LearningArea { get; set; }

        public virtual ICollection<CAM_CourseLearningSubArea> CamCourseLearningSubArea { get; set; }
    }
}
