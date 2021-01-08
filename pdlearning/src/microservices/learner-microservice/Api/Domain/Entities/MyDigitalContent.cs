using System;
using System.Linq.Expressions;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class MyDigitalContent : AuditedEntity, ISoftDelete, IHasStatus<MyDigitalContentStatus>, IHasCompletionDate
    {
        public static readonly int MaxReviewStatusLength = 1000;

        public Guid UserId { get; set; }

        public Guid DigitalContentId { get; set; }

        public MyDigitalContentStatus Status { get; set; }

        public DigitalContentType DigitalContentType { get; set; }

        public string Version { get; set; }

        public string ReviewStatus { get; set; }

        public double? ProgressMeasure { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DisenrollUtc { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime? ReminderSentDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public static Expression<Func<MyDigitalContent, bool>> FilterInProgressExpr()
        {
            return p => p.Status == MyDigitalContentStatus.InProgress;
        }

        public static Expression<Func<MyDigitalContent, bool>> FilterCompletedExpr()
        {
            return p => p.Status == MyDigitalContentStatus.Completed;
        }

        /// <summary>
        /// To check my digital content has not started.
        /// </summary>
        /// <returns>Returns true if the status is NotStarted, otherwise false.</returns>
        public bool IsNotStarted()
        {
            return Status == MyDigitalContentStatus.NotStarted;
        }
    }
}
