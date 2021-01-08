using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Helpers;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class ClassRun : FullAuditedEntity, ISoftDelete
    {
        private IEnumerable<Guid> _facilitatorIds = new List<Guid>();
        private IEnumerable<Guid> _coFacilitatorIds = new List<Guid>();

        public Guid CourseId { get; set; }

        public string ClassTitle { get; set; }

        public string ClassRunCode { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? PlanningStartTime { get; set; }

        public DateTime? PlanningEndTime { get; set; }

        public IEnumerable<Guid> FacilitatorIds
        {
            get => _facilitatorIds ?? new List<Guid>();

            set
            {
                _facilitatorIds = value;
                ClassRunInternalValues.RemoveAll(p => p.Type == ClassRunInternalValueType.FacilitatorIds);
                if (value != null)
                {
                    ClassRunInternalValues.AddRange(value.Select(p => ClassRunInternalValue.Create(Id, ClassRunInternalValueType.FacilitatorIds, p)).ToList());
                }
            }
        }

        public IEnumerable<Guid> CoFacilitatorIds
        {
            get => _coFacilitatorIds ?? new List<Guid>();

            set
            {
                _coFacilitatorIds = value;
                ClassRunInternalValues.RemoveAll(p => p.Type == ClassRunInternalValueType.CoFacilitatorIds);
                if (value != null)
                {
                    ClassRunInternalValues.AddRange(value.Select(p => ClassRunInternalValue.Create(Id, ClassRunInternalValueType.CoFacilitatorIds, p)).ToList());
                }
            }
        }

        public ContentStatus ContentStatus { get; set; } = ContentStatus.Draft;

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

        public string ExternalId { get; set; }

        public bool CourseCriteriaActivated { get; set; }

        public bool CourseAutomateActivated { get; set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public virtual ICollection<ClassRunInternalValue> ClassRunInternalValues { get; set; } = new List<ClassRunInternalValue>();

        /// <summary>
        /// This column to support filter equivalent to FacilitatorIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string FacilitatorIdsFullTextSearch
        {
            get => FacilitatorIds != null && FacilitatorIds.Any() ? JsonSerializer.Serialize(FacilitatorIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to CoFacilitatorIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string CoFacilitatorIdsFullTextSearch
        {
            get => CoFacilitatorIds != null && CoFacilitatorIds.Any() ? JsonSerializer.Serialize(CoFacilitatorIds) : null;
            set { }
        }

        public static Expression<Func<ClassRun, bool>> HasFacilitatorPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null
                        || UserRoles.IsSysAdministrator(userRoles)
                        || x.FacilitatorIds.Contains(userId.Value)
                        || x.CoFacilitatorIds.Contains(userId.Value);
        }

        public static Expression<Func<ClassRun, bool>> IsFacilitatorExpr(Guid? userId)
        {
            return x => x.FacilitatorIds.Contains(userId.Value)
                        || x.CoFacilitatorIds.Contains(userId.Value);
        }

        public static Expression<Func<ClassRun, bool>> IsNotCancelledExpr()
        {
            return x => x.Status == ClassRunStatus.Published
                || x.Status == ClassRunStatus.Unpublished;
        }

        public static ExpressionValidator<ClassRun> CanEditContentValidator(CourseEntity course, bool hasLearnerStarted = false)
        {
            Expression<Func<ClassRun, bool>> notPublishedContentExpr = x => x.ContentStatus != ContentStatus.Published;
            Expression<Func<ClassRun, bool>> notHasLearnerStartedExpr = x => !hasLearnerStarted;
            return new ExpressionValidator<ClassRun>(
                notHasLearnerStartedExpr.AndAlso(notPublishedContentExpr).AndAlso(p => course.IsNotArchived()),
                "Class content must be not published and not learner has started and course is not archived");
        }

        public static Expression<Func<ClassRun, bool>> StartedExpr()
        {
            return x => x.StartDateTime != null && x.StartDateTime.Value <= DateTimeHelper.EndOfTodayInSystemTimeZone().ToUtcFromSystemTimeZone();
        }

        public static Expression<Func<ClassRun, bool>> NotStartedExpr()
        {
            return StartedExpr().Not();
        }

        public static Expression<Func<ClassRun, bool>> EndedExpr()
        {
            return x => x.EndDateTime != null && x.EndDateTime.Value < DateTimeHelper.StartOfTodayInSystemTimeZone().ToUtcFromSystemTimeZone();
        }

        public static Expression<Func<ClassRun, bool>> NotEndedExpr()
        {
            return EndedExpr().Not();
        }

        public static Expression<Func<ClassRun, bool>> PublishedAndNotEndedExpr()
        {
            return PublishedExpr().AndAlso(NotEndedExpr());
        }

        public static Expression<Func<ClassRun, bool>> PublishedAndNotStartedExpr()
        {
            return PublishedExpr().AndAlso(NotStartedExpr());
        }

        public static Expression<Func<ClassRun, bool>> StartedAndIsNotELearningExpr(CourseEntity course)
        {
            return StartedExpr().AndAlso(IsNotELearningExpr(course));
        }

        public static Expression<Func<ClassRun, bool>> IsELearningExpr(CourseEntity course)
        {
            return classRun => course.IsELearning();
        }

        public static Expression<Func<ClassRun, bool>> IsNotELearningExpr(CourseEntity course)
        {
            return IsELearningExpr(course).Not();
        }

        public static ExpressionValidator<ClassRun> CanApproveRejectContentValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                classRun => classRun.ContentStatus == ContentStatus.PendingApproval && course.IsNotArchived(),
                "ContentStatus must be PendingApproval and course is not archived");
        }

        public static ExpressionValidator<ClassRun> CanPublishContentValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                classRun =>
                    (classRun.ContentStatus == ContentStatus.Approved || classRun.ContentStatus == ContentStatus.Unpublished) &&
                    course.IsNotArchived(),
                "Classrun ContentStatus must be Approved/Unpublished and course is not archived");
        }

        public static ExpressionValidator<ClassRun> CanUnpublishContentValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                classRun => classRun.ContentStatus == ContentStatus.Published && course.IsNotArchived(),
                "Classrun ContentStatus must be Published and course is not archived");
        }

        public static Expression<Func<ClassRun, bool>> EndedAndIsELearningExpr(CourseEntity course)
        {
            return EndedExpr().AndAlso(IsELearningExpr(course));
        }

        public static Expression<Func<ClassRun, bool>> NotEndedAndIsELearningExpr(CourseEntity course)
        {
            return EndedExpr().Not().AndAlso(IsELearningExpr(course));
        }

        public static ExpressionValidator<ClassRun> CanApplyIntoClassRunValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                PublishedAndNotStartedExpr()
                    .Or(PublishedAndNotEndedExpr().AndAlso(IsELearningExpr(course)))
                    .AndAlso(p => course.IsNotArchived()),
                "Classrun must be published and not started, or course is e-learning and classrun is published and not ended, and course is not archived");
        }

        public static Expression<Func<ClassRun, bool>> NeedSetExpiredRegistrationsClassRunExpr(CourseEntity course)
        {
            Expression<Func<ClassRun, bool>> cancelledExpr = x => x.Status == ClassRunStatus.Cancelled;
            if (course == null)
            {
                return cancelledExpr.Or(StartedExpr()).Or(EndedExpr());
            }

            return cancelledExpr.Or(StartedAndIsNotELearningExpr(course)).Or(EndedExpr());
        }

        public static Expression<Func<ClassRun, bool>> NeedSetAutoFailedRegistrationsClassRunExpr()
        {
            return EndedExpr();
        }

        public static Expression<Func<ClassRun, bool>> HasCancelClassRunPermissionExpr(
            CourseEntity course,
            Guid? currentUserId,
            List<string> currentUserRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return p => currentUserId == null ||
                        haveFullRight(course) ||
                        (course.HasAdministrationPermission(currentUserId, currentUserRoles, haveFullRight) &&
                            checkHasPermissionFn(CourseAdminManagementPermissionKeys.CancelClassRun));
        }

        public static Expression<Func<ClassRun, bool>> HasApprovalCancelClassRunPermissionExpr(
            CourseEntity course,
            Guid? currentUserId,
            List<string> currentUserRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return p => currentUserId == null ||
                        haveFullRight(course) ||
                        (course.HasApprovalPermission(currentUserId, currentUserRoles, haveFullRight) &&
                            checkHasPermissionFn(CourseAdminManagementPermissionKeys.CancellationClassRunRequestApproval));
        }

        public static Expression<Func<ClassRun, bool>> HasApprovalRescheduleClassRunPermissionExpr(
            CourseEntity course,
            Guid? currentUserId,
            List<string> currentUserRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return p => currentUserId == null ||
                        haveFullRight(course) ||
                        (course.HasApprovalPermission(currentUserId, currentUserRoles, haveFullRight) &&
                            checkHasPermissionFn(CourseAdminManagementPermissionKeys.RescheduledClassRunRequestApproval));
        }

        public static Expression<Func<ClassRun, bool>> HasRescheduleClassRunPermissionExpr(
            CourseEntity course,
            Guid? currentUserId,
            List<string> currentUserRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return p => currentUserId == null ||
                        haveFullRight(course) ||
                        (course.HasAdministrationPermission(currentUserId, currentUserRoles, haveFullRight) &&
                            checkHasPermissionFn(CourseAdminManagementPermissionKeys.RescheduleClassRun));
        }

        /// <summary>
        /// Receive 4 reminders at 4 weeks, 2 weeks, 1 week, 3 days prior to start date of the class.
        /// </summary>
        /// <returns>Filtered Expression.</returns>
        public static Expression<Func<ClassRun, bool>> CanRemindAdministratorConfirmRegistrationsExpr()
        {
            return p => (p.StartDateTime >= Clock.Now.AddDays(4 * 7) && p.StartDateTime < Clock.Now.AddDays((4 * 7) + 1)) ||
                        (p.StartDateTime >= Clock.Now.AddDays(2 * 7) && p.StartDateTime < Clock.Now.AddDays((2 * 7) + 1)) ||
                        (p.StartDateTime >= Clock.Now.AddDays(7) && p.StartDateTime < Clock.Now.AddDays(8)) ||
                        (p.StartDateTime >= Clock.Now.AddDays(3) && p.StartDateTime < Clock.Now.AddDays(4));
        }

        /// <summary>
        /// Receive 3 reminders at 2 weeks, 1 week, 3 days prior to start date of the class.
        /// </summary>
        /// <returns>Filtered Expression.</returns>
        public static Expression<Func<ClassRun, bool>> CanRemindCreatorConfirmRegistrationsExpr()
        {
            return p => (p.StartDateTime >= Clock.Now.AddDays(2 * 7) && p.StartDateTime < Clock.Now.AddDays((2 * 7) + 1)) ||
                        (p.StartDateTime >= Clock.Now.AddDays(7) && p.StartDateTime < Clock.Now.AddDays(8)) ||
                        (p.StartDateTime >= Clock.Now.AddDays(3) && p.StartDateTime < Clock.Now.AddDays(4));
        }

        public static Expression<Func<ClassRun, bool>> GetCanRemindConfirmRegistrationsExpr()
        {
            return CanRemindAdministratorConfirmRegistrationsExpr().Or(CanRemindCreatorConfirmRegistrationsExpr());
        }

        public static Expression<Func<ClassRun, bool>> PublishedExpr()
        {
            return p => p.Status == ClassRunStatus.Published;
        }

        /// <summary>
        /// Class run must be published and (not started Or (course is E-Learning and not ended)) class runs can be nominated.
        /// </summary>
        /// <param name="eLearningCourseIds">E-learning course ids related to class runs.</param>
        /// <returns>Can be nominated expression.</returns>
        public static Expression<Func<ClassRun, bool>> CanBeNominatedExpr(List<Guid> eLearningCourseIds)
        {
            Expression<Func<ClassRun, bool>> eLearningCourseIdsContainExpr = p => eLearningCourseIds.Contains(p.CourseId);
            return PublishedExpr().AndAlso(NotStartedExpr().Or(eLearningCourseIdsContainExpr.AndAlso(NotEndedExpr())));
        }

        public static bool HasCudPermission(
            Guid? userId,
            IEnumerable<string> userRoles,
            CourseEntity course,
            Func<CourseEntity, bool> haveCourseFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return userId == null
                   || course.HasAdministrationPermission(userId, userRoles, haveCourseFullRight)
                   || checkHasPermissionFn(CourseAdminManagementPermissionKeys.CreateEditClassRun);
        }

        public static Expression<Func<CourseEntity, bool>> HasPublishUnpublishPermissionExpr(
            Guid? userId,
            IEnumerable<string> userRoles,
            Func<string, bool> checkHasPermissionFn)
        {
            return x => userId == null
                        || UserRoles.IsSysAdministrator(userRoles)
                        || (checkHasPermissionFn(CourseAdminManagementPermissionKeys.PublishUnpublishClassRun)
                            && (x.FirstAdministratorId == userId || x.SecondAdministratorId == userId));
        }

        public static ExpressionValidator<ClassRun> CanCancelClassRunValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                p => course.IsNotArchived()
                     && course.Status != CourseStatus.Completed
                     && (!p.CancellationStatus.HasValue || p.CancellationStatus == ClassRunCancellationStatus.Rejected)
                     && p.RescheduleStatus != ClassRunRescheduleStatus.PendingApproval
                     && p.Started(),
                "Can only cancel a classrun which have course is not archived/completed, and class is started and has not been requested for cancelling.");
        }

        public static ExpressionValidator<ClassRun> CanApprovalRescheduleClassRunValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                p => course.IsNotArchived()
                     && course.Status != CourseStatus.Completed
                     && p.CancellationStatus != ClassRunCancellationStatus.PendingApproval,
                "Can only approve/reject a classrun reschedule request which have course is not archived/completed, and is pending approval for reschedule.");
        }

        public static ExpressionValidator<ClassRun> CanApprovalCancelClassRunValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                p => course.IsNotArchived()
                     && course.Status != CourseStatus.Completed
                     && p.CancellationStatus == ClassRunCancellationStatus.PendingApproval,
                "Can only approve/reject a classrun cancellation request which have course is not archived/completed, and is pending approval for cancellation.");
        }

        public static ExpressionValidator<ClassRun> CanRescheduleClassRunValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                p => course.IsNotArchived()
                     && course.Status != CourseStatus.Completed
                     && p.RescheduleStatus != ClassRunRescheduleStatus.PendingApproval
                     && (!p.CancellationStatus.HasValue || p.CancellationStatus == ClassRunCancellationStatus.Rejected)
                     && p.ApplicationStarted(),
                "Can only reschedule a classrun which have course is not archived/completed, or class is application started and has not been requested for rescheduling/cancelling and has not been cancelled.");
        }

        public static ExpressionValidator<ClassRun> CanPublishValidator(CourseEntity course)
        {
            return new ExpressionValidator<ClassRun>(
                p => course.IsNotArchived()
                     && course.Status != CourseStatus.Completed
                     && p.Status == ClassRunStatus.Unpublished
                     && p.PlanningStartTime != null
                     && p.PlanningEndTime != null,
                "Can only publish a classrun which have course is not archived/completed, or class is unpublished and have planning start/end time.");
        }

        public static ExpressionValidator<ClassRun> CanBeNominatedValidator(CourseEntity course)
        {
            if (course.IsArchived() || course.Status != CourseStatus.Published)
            {
                return new ExpressionValidator<ClassRun>(
                    p => false,
                    "Can only nominate learner into class of not-archived and published course.");
            }

            Expression<Func<ClassRun, bool>> isCourseELearningExpr = p => course.LearningMode == MetadataTagConstants.ELearningTagId;
            var finalExpr = PublishedExpr().AndAlso(NotStartedExpr().Or(isCourseELearningExpr.AndAlso(NotEndedExpr())));
            return new ExpressionValidator<ClassRun>(
                finalExpr,
                "Can only nominate learner into class which is published, not started or not ended for e-learning course.");
        }

        public bool CanBeNominated(List<Guid> eLearningCourseIds)
        {
            return CanBeNominatedExpr(eLearningCourseIds).Compile()(this);
        }

        public bool Published()
        {
            return PublishedExpr().Compile()(this);
        }

        public bool CanUpdate(CourseEntity course)
        {
            return !Started() && Status == ClassRunStatus.Unpublished && course.IsNotArchived();
        }

        public bool HasFacilitatorPermission(Guid? userId, IEnumerable<string> userRoles, bool haveFullRight)
        {
            return HasFacilitatorPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight;
        }

        public bool IsFacilitator(Guid? userId)
        {
            return IsFacilitatorExpr(userId).Compile()(this);
        }

        public Validation ValidateCanEditContent(CourseEntity course, bool hasLearnerStarted = false)
        {
            return CanEditContentValidator(course, hasLearnerStarted).Validate(this);
        }

        public bool Started()
        {
            return StartedExpr().Compile()(this);
        }

        public bool Ended()
        {
            return EndedExpr().Compile()(this);
        }

        public bool PublishedAndNotEnded()
        {
            return PublishedAndNotEndedExpr().Compile()(this);
        }

        public bool PublishedAndNotStarted()
        {
            return PublishedAndNotStartedExpr().Compile()(this);
        }

        public bool StartedAndIsNotELearning(CourseEntity course)
        {
            return StartedAndIsNotELearningExpr(course).Compile()(this);
        }

        public bool ApplicationStarted()
        {
            return ApplicationStartDate != null && ApplicationStartDate.Value <= Clock.Now;
        }

        public Validation ValidateCanApproveRejectContent(CourseEntity course)
        {
            return CanApproveRejectContentValidator(course).Validate(this);
        }

        public Validation ValidateCanPublishContent(CourseEntity course)
        {
            return CanPublishContentValidator(course).Validate(this);
        }

        public Validation ValidateCanUnpublishContent(CourseEntity course)
        {
            return CanUnpublishContentValidator(course).Validate(this);
        }

        public bool CanRemindAdministratorConfirmRegistrations()
        {
            return CanRemindAdministratorConfirmRegistrationsExpr().Compile()(this);
        }

        public bool CanRemindCreatorConfirmRegistrations()
        {
            return CanRemindCreatorConfirmRegistrationsExpr().Compile()(this);
        }

        public Validation ValidateCanAddParticipants(CourseEntity course)
        {
            return Validation.HarvestErrors(
                course.ValidateNotArchived(),
                Validation.ValidIf(this.Published() && !course.IsMicroLearning(), "Class run must be published and not for micro learning courses."));
        }

        public Validation ValidateCanReschedule(CourseEntity course)
        {
            return Validation.ValidIf(course.Status != CourseStatus.Completed, "Cannot reschedule this classrun since its course is completed");
        }

        public bool NotEndedAndIsELearning(CourseEntity course)
        {
            return NotEndedAndIsELearningExpr(course).Compile()(this);
        }

        public Validation ValidateCanApplyIntoClassRun(CourseEntity course)
        {
            return CanApplyIntoClassRunValidator(course).Validate(this);
        }

        public bool NeedSetExpiredRegistrationsClassRun(CourseEntity course)
        {
            return NeedSetExpiredRegistrationsClassRunExpr(course).Compile()(this);
        }

        public bool NeedSetAutoFailedRegistrationsClassRun()
        {
            return NeedSetAutoFailedRegistrationsClassRunExpr().Compile()(this);
        }

        public List<Guid> GetCanRemindConfirmRegistrationsUserIds(CourseEntity course)
        {
            return new List<Guid>()
                .Concat(CanRemindAdministratorConfirmRegistrations() ? course.GetAdministratorIds() : new List<Guid>())
                .Concat(CanRemindCreatorConfirmRegistrations() ? course.GetAllCreatorIds() : new List<Guid>())
                .ToList();
        }

        public bool HasContentCustomizationPermission(
            CourseEntity courseEntity,
            Guid? userId,
            List<string> userRoles,
            Func<CourseEntity, bool> haveFullRight)
        {
            return courseEntity.HasContentCreatorPermission(userId, userRoles, haveFullRight) ||
                   courseEntity.HasFacilitatorPermission(userId, userRoles, haveFullRight) ||
                   courseEntity.HasAdministrationPermission(userId, userRoles, haveFullRight) ||
                   courseEntity.HasApprovalPermission(userId, userRoles, haveFullRight) ||
                   HasFacilitatorPermission(userId, userRoles, haveFullRight(courseEntity));
        }

        public IEnumerable<Guid> GetAssignmentManagementUserIds()
        {
            return new List<Guid>()
                .Concat(FacilitatorIds)
                .Concat(CoFacilitatorIds ?? new List<Guid>());
        }

        public bool HasCancelClassRunPermission(
            CourseEntity course,
            Guid? currentUserId,
            List<string> currentUserRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return HasCancelClassRunPermissionExpr(course, currentUserId, currentUserRoles, haveFullRight, checkHasPermissionFn).Compile()(this);
        }

        public bool HasApprovalCancelClassRunPermission(
            CourseEntity course,
            Guid? currentUserId,
            List<string> currentUserRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return HasApprovalCancelClassRunPermissionExpr(course, currentUserId, currentUserRoles, haveFullRight, checkHasPermissionFn).Compile()(this);
        }

        public bool HasApprovalRescheduleClassRunPermission(
            CourseEntity course,
            Guid? currentUserId,
            List<string> currentUserRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return HasApprovalRescheduleClassRunPermissionExpr(course, currentUserId, currentUserRoles, haveFullRight, checkHasPermissionFn).Compile()(this);
        }

        public bool HasRescheduleClassRunPermission(
            CourseEntity course,
            Guid? currentUserId,
            List<string> currentUserRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return HasRescheduleClassRunPermissionExpr(course, currentUserId, currentUserRoles, haveFullRight, checkHasPermissionFn).Compile()(this);
        }

        public Validation ValidateCanCancelClassRun(CourseEntity course)
        {
            return CanCancelClassRunValidator(course).Validate(this);
        }

        public Validation ValidateCanApprovalRescheduleClassRun(CourseEntity course)
        {
            return CanApprovalRescheduleClassRunValidator(course).Validate(this);
        }

        public Validation ValidateCanApprovalCancelClassRun(CourseEntity course)
        {
            return CanApprovalCancelClassRunValidator(course).Validate(this);
        }

        public Validation ValidateCanRescheduleClassRun(CourseEntity course)
        {
            return CanRescheduleClassRunValidator(course).Validate(this);
        }

        public Validation ValidateCanPublish(CourseEntity course)
        {
            return CanPublishValidator(course).Validate(this);
        }

        public Validation ValidateCanBeNominated(CourseEntity course)
        {
            return CanBeNominatedValidator(course).Validate(this);
        }
    }
}
