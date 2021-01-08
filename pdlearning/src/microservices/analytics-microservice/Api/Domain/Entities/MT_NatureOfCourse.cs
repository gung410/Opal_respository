using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_NatureOfCourse
    {
        public MT_NatureOfCourse()
        {
            CamCourse = new HashSet<CAM_Course>();
        }

        public Guid NatureOfCourseId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public virtual ICollection<CAM_Course> CamCourse { get; set; }
    }
}
