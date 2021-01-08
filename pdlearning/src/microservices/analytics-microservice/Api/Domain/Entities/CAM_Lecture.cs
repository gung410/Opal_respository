using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_Lecture
    {
        public CAM_Lecture()
        {
            CamLectureContent = new HashSet<CAM_LectureContent>();
            LearnerLecturesInUserCourse = new HashSet<Learner_LecturesInUserCourse>();
            LearnerUserLectureInCourse = new HashSet<Learner_UserLectureInCourse>();
        }

        public Guid LectureId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid? SectionId { get; set; }

        public Guid CourseId { get; set; }

        public string LectureName { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public int? Order { get; set; }

        public Guid? ClassRunId { get; set; }

        public string ExternalId { get; set; }

        public virtual CAM_ClassRun ClassRun { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual CAM_Section Section { get; set; }

        public virtual ICollection<CAM_LectureContent> CamLectureContent { get; set; }

        public virtual ICollection<Learner_LecturesInUserCourse> LearnerLecturesInUserCourse { get; set; }

        public virtual ICollection<Learner_UserLectureInCourse> LearnerUserLectureInCourse { get; set; }
    }
}
