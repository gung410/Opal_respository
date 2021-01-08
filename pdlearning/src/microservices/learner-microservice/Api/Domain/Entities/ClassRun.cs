using System;
using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from ClassRun table on the CAM module.
    /// </summary>
    public class ClassRun : AuditedEntity
    {
        public Guid CourseId { get; set; }

        public string ClassTitle { get; set; }

        public string ClassRunCode { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? PlanningStartTime { get; set; }

        public DateTime? PlanningEndTime { get; set; }

        public IEnumerable<Guid> FacilitatorIds { get; set; }

        public IEnumerable<Guid> CoFacilitatorIds { get; set; }

        public ContentStatus ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public int MinClassSize { get; set; }

        public int MaxClassSize { get; set; }

        public DateTime? ApplicationStartDate { get; set; }

        public DateTime? ApplicationEndDate { get; set; }

        public ClassRunStatus Status { get; set; }

        public Guid? ClassRunVenueId { get; set; }

        public ClassRunCancellationStatus? CancellationStatus { get; set; }

        public ClassRunRescheduleStatus? RescheduleStatus { get; set; }

        public DateTime? RescheduleStartDateTime { get; set; }

        public DateTime? RescheduleEndDateTime { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }
    }
}
