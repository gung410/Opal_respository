using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
    public class ClassRunChangeMessage : IMQMessageHasCreatedDate
    {
        public Guid Id { get; set; }

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

        public string Reason { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? MessageCreatedDate { get; set; }

        /// <summary>
        /// To check the reschedule class run status has been approved by course admin.
        /// </summary>
        /// <returns>Returns true if the reschedule status is Approved
        /// and the cancellation status is null, otherwise false.</returns>
        public bool IsApprovedClassRescheduled()
        {
            return RescheduleStatus == ClassRunRescheduleStatus.Approved
                && CancellationStatus == null;
        }

        /// <summary>
        /// To check the cancellation class run status has been approved by course admin.
        /// </summary>
        /// <returns>Returns true if the cancellation status is Approved
        /// and the cancellation status is null or Approved.</returns>
        public bool IsApprovedClassCancelled()
        {
            return CancellationStatus == ClassRunCancellationStatus.Approved
                   && (RescheduleStatus == null
                       || RescheduleStatus == ClassRunRescheduleStatus.Approved);
        }
    }
}
