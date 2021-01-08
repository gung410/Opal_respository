using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class LectureInMyCourse : AuditedEntity, ISoftDelete
    {
        public static readonly int MaxReviewStatusLength = 1000;

        public Guid MyCourseId { get; set; }

        public Guid LectureId { get; set; }

        public Guid UserId { get; set; }

        public LectureStatus Status { get; set; }

        public string ReviewStatus { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public string ExternalId { get; set; }

        public string Version { get; set; }
    }
}
