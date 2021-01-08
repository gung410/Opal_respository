using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from the Assignment table on the CAM module.
    /// </summary>
    public class Assignment : AuditedEntity
    {
        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public AssignmentType Type { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public void Update(
            Guid courseId,
            Guid? classRunId,
            string title,
            AssignmentType type,
            DateTime? changedDate,
            Guid? changedBy)
        {
            CourseId = courseId;
            ClassRunId = classRunId;
            Title = title;
            Type = type;
            ChangedDate = changedDate;
            ChangedBy = changedBy;
        }
    }
}
