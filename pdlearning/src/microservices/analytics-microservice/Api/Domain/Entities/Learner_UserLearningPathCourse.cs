using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserLearningPathCourse
    {
        public Guid UserLearningPathCoursesId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? CreatedDateKey { get; set; }

        public int? CreatedTimeKey { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid UserLearningPathId { get; set; }

        public Guid CourseId { get; set; }

        public int? Order { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Learner_UserLearningPath UserLearningPath { get; set; }
    }
}
