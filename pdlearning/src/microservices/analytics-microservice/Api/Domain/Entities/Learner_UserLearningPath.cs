using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserLearningPath
    {
        public Learner_UserLearningPath()
        {
            LearnerUserLearningPathCourses = new HashSet<Learner_UserLearningPathCourse>();
        }

        public Guid UserLearningPathId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string Title { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsPublic { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }

        public virtual ICollection<Learner_UserLearningPathCourse> LearnerUserLearningPathCourses { get; set; }
    }
}
