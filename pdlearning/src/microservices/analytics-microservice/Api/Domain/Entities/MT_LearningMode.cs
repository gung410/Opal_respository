using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_LearningMode
    {
        public MT_LearningMode()
        {
            CamCourse = new HashSet<CAM_Course>();
        }

        public Guid Id { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public virtual ICollection<CAM_Course> CamCourse { get; set; }
    }
}
