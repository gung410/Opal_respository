using System;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class CourseReview : AuditedEntity, ISoftDelete
    {
        public static readonly int MaxVersionLength = 100;
        public static readonly int MaxItemNameLength = 500;
        public static readonly int MaxUserFullNameLength = 500;
        public static readonly int MaxCommentTitleLength = 100;
        public static readonly int MaxCommentContentLength = 2000;

        public Guid? ParentCommentId { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public string Version { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? LectureId { get; set; }

        public string ItemName { get; set; }

        public string UserFullName { get; set; }

        public string CommentTitle { get; set; }

        public string CommentContent { get; set; }

        public double? Rate { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }
    }
}
