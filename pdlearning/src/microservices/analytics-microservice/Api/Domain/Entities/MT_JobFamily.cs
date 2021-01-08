using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_JobFamily
    {
        public MT_JobFamily()
        {
            CamCourse = new HashSet<CAM_Course>();
            CamCourseJobFamily = new HashSet<CAM_CourseJobFamily>();
            SamUserJobFamily = new HashSet<SAM_UserJobFamily>();
        }

        public Guid JobFamilyId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public virtual ICollection<CAM_Course> CamCourse { get; set; }

        public virtual ICollection<CAM_CourseJobFamily> CamCourseJobFamily { get; set; }

        public virtual ICollection<SAM_UserJobFamily> SamUserJobFamily { get; set; }
    }
}
