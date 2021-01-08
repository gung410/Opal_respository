using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_LecturesInUserCourse
    {
        public Learner_LecturesInUserCourse()
        {
            LearnerUserLearningPackages = new HashSet<Learner_UserLearningPackage>();
        }

        public Guid LecturesInUserCourseId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid UserCourseId { get; set; }

        public Guid LectureId { get; set; }

        public Guid UserId { get; set; }

        public string Status { get; set; }

        public string ReviewStatus { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public string ExternalId { get; set; }

        public string Version { get; set; }

        public virtual CAM_Lecture Lecture { get; set; }

        public virtual Learner_UserCourse UserCourse { get; set; }

        public virtual ICollection<Learner_UserLearningPackage> LearnerUserLearningPackages { get; set; }
    }
}
