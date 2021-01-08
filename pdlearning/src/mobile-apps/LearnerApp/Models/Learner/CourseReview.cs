using System;

namespace LearnerApp.Models
{
    public class CourseReview
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string CourseId { get; set; }

        public string ParentCommentId { get; set; }

        public string Version { get; set; }

        public string SectionId { get; set; }

        public string LectureId { get; set; }

        public string ItemName { get; set; }

        public string UserFullName { get; set; }

        public string CommentTitle { get; set; }

        public string CommentContent { get; set; }

        public double Rate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public string ChangedBy { get; set; }
    }
}
