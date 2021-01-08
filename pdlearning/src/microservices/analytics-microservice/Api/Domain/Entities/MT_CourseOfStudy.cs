using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_CourseOfStudy
    {
        public MT_CourseOfStudy()
        {
            CamCourseCourseOfStudy = new HashSet<CAM_CourseCourseOfStudy>();
            SamUserCourseOfStudy = new HashSet<SAM_UserCourseOfStudy>();
        }

        public Guid CourseOfStudyId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual ICollection<CAM_CourseCourseOfStudy> CamCourseCourseOfStudy { get; set; }

        public virtual ICollection<SAM_UserCourseOfStudy> SamUserCourseOfStudy { get; set; }
    }
}
