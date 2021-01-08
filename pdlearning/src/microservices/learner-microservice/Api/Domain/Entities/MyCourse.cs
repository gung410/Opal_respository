using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class MyCourse : AuditedEntity, ISoftDelete, IHasStatus<MyCourseStatus>, IHasCompletionDate
    {
        public static readonly int MaxVersionLength = 100;
        public static readonly int MaxReviewStatusLength = 1000;

        public Guid CourseId { get; set; }

        public Guid UserId { get; set; }

        public MyCourseStatus Status { get; set; }

        public string ReviewStatus { get; set; }

        public double? ProgressMeasure { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DisenrollUtc { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime? ReminderSentDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? CurrentLecture { get; set; }

        public LearningCourseType CourseType { get; set; }

        public RegistrationStatus? MyRegistrationStatus { get; set; }

        public Guid? ResultId { get; set; }

        public WithdrawalStatus? MyWithdrawalStatus { get; set; }

        public string ExternalId { get; set; }

        public DisplayStatus? DisplayStatus { get; set; }

        public Guid? RegistrationId { get; set; }

        public string Version { get; set; }

        public bool HasContentChanged { get; set; }

        // TODO: need to be removed because it is no longer in use
        public bool? PostCourseEvaluationFormCompleted { get; set; }

        public static Expression<Func<MyCourse, bool>> FilterMicroLearningExpr()
        {
            return p => p.CourseType == LearningCourseType.Microlearning;
        }

        public static Expression<Func<MyCourse, bool>> FilterRegisteredExpr()
        {
            return p => p.MyRegistrationStatus != null;
        }

        public static Expression<Func<MyCourse, bool>> FilterUpcomingExpr()
        {
            return p =>
                (p.MyRegistrationStatus == RegistrationStatus.ConfirmedByCA
                 || p.MyRegistrationStatus == RegistrationStatus.OfferConfirmed)
                && p.MyWithdrawalStatus != WithdrawalStatus.Withdrawn
                && p.Status == MyCourseStatus.NotStarted;
        }

        public static Expression<Func<MyCourse, bool>> FilterInProgressExpr()
        {
            return p => p.Status == MyCourseStatus.InProgress || p.Status == MyCourseStatus.Passed;
        }

        public static Expression<Func<MyCourse, bool>> FilterCompletedExpr()
        {
            return p => p.Status == MyCourseStatus.Completed || p.Status == MyCourseStatus.Failed;
        }

        public static Expression<Func<MyCourse, bool>> FilterByUserIdExpr(Guid userId)
        {
            return p => p.UserId == userId;
        }

        public static Expression<Func<MyCourse, bool>> FilterByStatusExpr(List<MyCourseStatus> statuses)
        {
            return p => statuses.Contains(p.Status);
        }

        public static Expression<Func<MyCourse, bool>> FilterByCourseTypeExpr(LearningCourseType courseType)
        {
            return p => p.CourseType == courseType;
        }

        public static Expression<Func<MyCourse, bool>> FilterByCourseIdsExpr(List<Guid> courseIds)
        {
            return p => courseIds.Contains(p.CourseId);
        }

        /// <summary>
        /// The course is MicroLearning.
        /// </summary>
        /// <returns>Returns true if the course type is MicroLearning,
        /// otherwise false.</returns>
        public bool IsMicroLearning()
        {
            return FilterMicroLearningExpr().Compile()(this);
        }

        /// <summary>
        /// Notify users of their latest action at which step in the workflow.
        /// Actions include registration class, change class, withdraw class.
        /// </summary>
        /// <param name="displayStatus">
        /// Display status as value object <see cref="ValueObject.DisplayStatus"/>.</param>
        public void SetDisplayStatus(DisplayStatus displayStatus)
        {
            DisplayStatus = displayStatus;
        }

        /// <summary>
        /// Notify users that their registration at which step in the workflow.
        /// </summary>
        /// <param name="registrationStatus">
        /// Registration status as value object <see cref="ValueObject.RegistrationStatus"/>.</param>
        public void SetMyRegistrationStatus(RegistrationStatus registrationStatus)
        {
            MyRegistrationStatus = registrationStatus;
        }

        public void SetRegistrationId(Guid registrationId)
        {
            RegistrationId = registrationId;
        }

        /// <summary>
        /// To check my course has completed or failed.
        /// </summary>
        /// <returns>Returns true if the status is completed or failed,
        /// otherwise false.</returns>
        public bool IsFinishedLearning()
        {
            return Status == MyCourseStatus.Failed || Status == MyCourseStatus.Completed;
        }

        /// <summary>
        /// The course has started.
        /// </summary>
        /// <returns>Returns true if the status is NotStarted, otherwise false.</returns>
        public bool IsStarted()
        {
            return Status != MyCourseStatus.NotStarted;
        }
    }
}
