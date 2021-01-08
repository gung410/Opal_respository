using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserReview
    {
        public Guid UserReviewsId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid? ParentCommentId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public Guid UserId { get; set; }

        public string DepartmentId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? DigitalContentId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string ItemType { get; set; }

        public string Version { get; set; }

        public string ItemName { get; set; }

        public string UserFullName { get; set; }

        public string CommentTitle { get; set; }

        public string CommentContent { get; set; }

        public double? Rate { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public virtual CAM_ClassRun ClassRun { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual SAM_Department Department { get; set; }

        public virtual CCPM_DigitalContent DigitalContent { get; set; }
    }
}
