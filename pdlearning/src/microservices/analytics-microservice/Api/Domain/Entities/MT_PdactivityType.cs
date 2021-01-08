using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_PdactivityType
    {
        public MT_PdactivityType()
        {
            CamCourse = new HashSet<CAM_Course>();
        }

        public Guid PdactivityTypeId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public virtual ICollection<CAM_Course> CamCourse { get; set; }
    }
}
