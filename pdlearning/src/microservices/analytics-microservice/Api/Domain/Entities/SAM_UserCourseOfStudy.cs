using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserCourseOfStudy
    {
        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public Guid CourseOfStudyId { get; set; }

        public virtual MT_CourseOfStudy CourseOfStudy { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
