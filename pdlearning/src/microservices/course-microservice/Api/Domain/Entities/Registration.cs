using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Course.Application.Constants;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class Registration : FullAuditedEntity, ISoftDelete
    {
        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public RegistrationType RegistrationType { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public RegistrationStatus Status { get; set; }

        public DateTime? LastStatusChangedDate { get; set; }

        public WithdrawalStatus? WithdrawalStatus { get; set; }

        public DateTime? WithdrawalRequestDate { get; set; }

        public ClassRunChangeStatus? ClassRunChangeStatus { get; set; }

        public DateTime? ClassRunChangeRequestedDate { get; set; }

        public Guid? ClassRunChangeId { get; set; }

        public Guid ApprovingOfficer { get; set; }

        public Guid? AlternativeApprovingOfficer { get; set; }

        public DateTime? ApprovingDate { get; set; }

        public DateTime? OfferSentDate { get; set; }

        public Guid? AdministratedBy { get; set; }

        public DateTime? AdministrationDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public string ExternalId { get; set; }

        public double? LearningContentProgress { get; set; }

        public LearningStatus LearningStatus { get; set; } = LearningStatus.NotStarted;

        public bool PostCourseEvaluationFormCompleted { get; set; } = false;

        public DateTime? LearningCompletedDate { get; set; }

        public Guid? CompleteCourseECertificateId { get; set; }

        public CourseCriteriaLearnerViolation CourseCriteriaViolation { get; set; }

        public bool IsExpired { get; set; }

        public bool? CourseCriteriaViolated
        {
            get => CourseCriteriaViolation == null ? (bool?)null : CourseCriteriaViolation.IsViolated;
            set { }
        }

        public bool? CourseCriteriaOverrided { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? SendPostCourseSurveyDate { get; set; }

        public bool IsParticipant => IsParticipantExpr().Compile()(this);

        [JsonIgnore]
        public virtual RegistrationECertificate ECertificate { get; set; }

        public static Expression<Func<Registration, bool>> IsPendingConfirmationExpr()
        {
            return IsExistedExpr().AndAlso(x => x.Status == RegistrationStatus.Approved);
        }

        public static Expression<Func<Registration, bool>> IsClassRunRegistrationExpr()
        {
            return x =>
                x.WithdrawalStatus != Enums.WithdrawalStatus.Withdrawn
                && x.ClassRunChangeStatus != Enums.ClassRunChangeStatus.ConfirmedByCA &&
                (x.Status == RegistrationStatus.Approved || x.Status == RegistrationStatus.RejectedByCA);
        }

        public static Expression<Func<Registration, bool>> IsWaitlistExpr()
        {
            return x =>
                x.WithdrawalStatus != Enums.WithdrawalStatus.Withdrawn
                && x.ClassRunChangeStatus != Enums.ClassRunChangeStatus.ConfirmedByCA
                && (x.Status == RegistrationStatus.WaitlistConfirmed
                    || x.Status == RegistrationStatus.WaitlistRejected
                    || x.Status == RegistrationStatus.OfferRejected
                    || x.Status == RegistrationStatus.OfferPendingApprovalByLearner
                    || x.Status == RegistrationStatus.WaitlistPendingApprovalByLearner
                    || x.Status == RegistrationStatus.AddedByCAClassfull);
        }

        public static Expression<Func<Registration, bool>> IsParticipantExpr()
        {
            return IsExistedExpr().AndAlso(x =>
                (x.Status == RegistrationStatus.ConfirmedByCA || x.Status == RegistrationStatus.OfferConfirmed));
        }

        public static Expression<Func<Registration, bool>> IsPendingApprovalByLearnerExpr()
        {
            return IsExistedExpr().AndAlso(x =>
                x.Status == RegistrationStatus.WaitlistPendingApprovalByLearner ||
                x.Status == RegistrationStatus.OfferPendingApprovalByLearner);
        }

        public static Expression<Func<Registration, bool>> IsLearningInProgressParticipantExpr()
        {
            return IsParticipantExpr().AndAlsoNot(IsLearningFinishedExpr());
        }

        public static Expression<Func<Registration, bool>> IsChangeClassExpr()
        {
            return x => x.ClassRunChangeStatus == Enums.ClassRunChangeStatus.Approved
                || x.ClassRunChangeStatus == Enums.ClassRunChangeStatus.ConfirmedByCA
                || x.ClassRunChangeStatus == Enums.ClassRunChangeStatus.RejectedByCA;
        }

        public static Expression<Func<Registration, bool>> InProgressExpr()
        {
            return IsExistedExpr().AndAlso(x =>
                x.Status != RegistrationStatus.Rejected
                && x.Status != RegistrationStatus.RejectedByCA
                && x.Status != RegistrationStatus.WaitlistRejected
                && x.Status != RegistrationStatus.OfferRejected);
        }

        public static Expression<Func<Registration, bool>> IsPendingExpr()
        {
            return IsExistedExpr().AndAlso(x =>
                x.Status == RegistrationStatus.OfferPendingApprovalByLearner
                || x.Status == RegistrationStatus.Approved
                || x.Status == RegistrationStatus.PendingConfirmation
                || x.Status == RegistrationStatus.WaitlistPendingApprovalByLearner
                || x.Status == RegistrationStatus.WaitlistConfirmed);
        }

        public static Expression<Func<Registration, bool>> IsCompletedExpr()
        {
            return x => x.LearningStatus == LearningStatus.Completed;
        }

        public static Expression<Func<Registration, bool>> IsFailedExpr()
        {
            return x => x.LearningStatus == LearningStatus.Failed;
        }

        public static Expression<Func<Registration, bool>> IsPendingAdministrationExpr()
        {
            return IsExistedExpr().AndAlso(x => x.Status != RegistrationStatus.RejectedByCA
                                                && x.Status != RegistrationStatus.Rejected
                                                && x.Status != RegistrationStatus.OfferConfirmed
                                                && x.Status != RegistrationStatus.PendingConfirmation
                                                && x.Status != RegistrationStatus.ConfirmedByCA
                                                && x.Status != RegistrationStatus.AddedByCAClassfull
                                                && x.Status != RegistrationStatus.AddedByCAConflict);
        }

        public static Expression<Func<Registration, bool>> IsRejectedExpr()
        {
            return x => x.Status == RegistrationStatus.RejectedByCA
                        || x.Status == RegistrationStatus.Rejected
                        || x.Status == RegistrationStatus.OfferRejected
                        || x.Status == RegistrationStatus.WaitlistRejected
                        || x.WithdrawalStatus == Enums.WithdrawalStatus.Withdrawn;
        }

        public static Expression<Func<Registration, bool>> HasManagePermissionExpr(
            CourseEntity course,
            ClassRun classRun,
            Guid? userId,
            List<string> userRoles,
            Func<CourseEntity, bool> haveCourseFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return x => userId == null ||
                        UserRoles.IsSysAdministrator(userRoles) ||
                        ((checkHasPermissionFn(CourseAdminManagementPermissionKeys.ManageRegistrations) || checkHasPermissionFn(PDPMPermissionKeys.ManageRegistrations)) &&
                         (course.HasAdministrationPermission(userId, userRoles, haveCourseFullRight) ||
                          course.HasFacilitatorPermission(userId, userRoles, haveCourseFullRight) ||
                          classRun.HasFacilitatorPermission(userId, userRoles, haveCourseFullRight(course)) ||
                          x.ApprovingOfficer == userId ||
                          x.AlternativeApprovingOfficer == userId));
        }

        public static Expression<Func<Registration, bool>> HasCreateWithdrawRequestPermissionExpr(
            Guid? userId,
            List<string> userRoles)
        {
            return x => userId == null ||
                        UserRoles.IsSysAdministrator(userRoles) ||
                        x.UserId == userId;
        }

        public static Expression<Func<Registration, bool>> HasApproveRejectWaitlistPermissionExpr(
            Guid? userId,
            List<string> userRoles)
        {
            return x => userId == null ||
                        UserRoles.IsSysAdministrator(userRoles) ||
                        x.UserId == userId;
        }

        public static Expression<Func<Registration, bool>> HasApproveRejectOfferPermissionExpr(
            Guid? userId,
            List<string> userRoles)
        {
            return x => userId == null ||
                        UserRoles.IsSysAdministrator(userRoles) ||
                        x.UserId == userId;
        }

        public static Expression<Func<Registration, bool>> HasChangeClassRunPermissionExpr(CourseEntity course, Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return x => userId == null || x.UserId == userId || course.HasAdministrationPermission(userId, userRoles, haveFullRight);
        }

        public static Expression<Func<Registration, bool>> CanChangeClassRunExpr(CourseEntity course)
        {
            return CanChangeClassRunValidator(course).IsValidExpression;
        }

        public static ExpressionValidator<Registration> CanChangeClassRunValidator(CourseEntity course)
        {
            Expression<Func<Registration, bool>> notNull = x => x != null;
            var canChangeClassRunExpr = notNull
                    .AndAlso(x => x.Status != RegistrationStatus.PendingConfirmation)
                    .AndAlso(x => x.Status != RegistrationStatus.Rejected)
                    .AndAlso(x => x.Status != RegistrationStatus.RejectedByCA)
                    .AndAlso(x => x.LearningStatus == LearningStatus.NotStarted)
                    .AndAlso(x => !x.ClassRunChangeStatus.HasValue ||
                                  x.ClassRunChangeStatus == Enums.ClassRunChangeStatus.Rejected ||
                                  x.ClassRunChangeStatus == Enums.ClassRunChangeStatus.RejectedByCA)
                    .AndAlso(x => course.IsNotArchived())
                    .AndAlso(x => !x.IsExpired);
            return new ExpressionValidator<Registration>(
                canChangeClassRunExpr,
                "Registration must not Pending Confirmation, Rejected, Withdrawing, Withdrew or Expired");
        }

        public static Expression<Func<Registration, bool>> IsLearningFinishedExpr()
        {
            return x => x.LearningStatus == LearningStatus.Passed ||
                        x.LearningStatus == LearningStatus.Failed ||
                        x.LearningStatus == LearningStatus.Completed;
        }

        public static Expression<Func<Registration, bool>> IsNotExpiredExpr()
        {
            return x => x.IsExpired == false;
        }

        public static Expression<Func<Registration, bool>> CanBeSetExpiredExpr()
        {
            return IsExistedExpr().AndAlsoNot(IsParticipantExpr()).AndAlsoNot(IsRejectedExpr());
        }

        public static Expression<Func<Registration, bool>> CanBeSetAutoFailedExpr()
        {
            return IsParticipantExpr().AndAlso(p => p.LearningStatus == LearningStatus.NotStarted);
        }

        public static Expression<Func<Registration, bool>> IsNotAbleToBeNominatedExpr()
        {
            return x => x.LearningStatus == LearningStatus.NotStarted
                        || x.LearningStatus == LearningStatus.InProgress
                        || x.LearningStatus == LearningStatus.Passed
                        || (x.LearningStatus == LearningStatus.Failed && x.PostCourseEvaluationFormCompleted == false);
        }

        public static Expression<Func<Registration, bool>> CanCompletePostCourseEvaluationExpr(CourseEntity course)
        {
            return x => ((x.LearningStatus == LearningStatus.Failed && !x.PostCourseEvaluationFormCompleted) || x.LearningStatus == LearningStatus.Passed) &&
                        course.IsNotArchived();
        }

        public static Expression<Func<Registration, bool>> CanTriggerPostCourseEvaluationExpr()
        {
            return x => (!x.SendPostCourseSurveyDate.HasValue && (x.LearningStatus == LearningStatus.Failed || x.LearningStatus == LearningStatus.Passed));
        }

        public static Expression<Func<Registration, bool>> IsSlotTakingExpr()
        {
            return IsExistedExpr()
                .AndAlso(IsParticipantExpr().Or(x => x.Status == RegistrationStatus.OfferPendingApprovalByLearner));
        }

        public static bool CanManageRegistrations(Guid? userId, CourseEntity course, ClassRun classRun, List<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return
                course.IsNotArchived() && CanViewManagedRegistrations(userId, course, classRun, userRoles, haveFullRight);
        }

        public static bool CanViewManagedRegistrations(Guid? userId, CourseEntity course, ClassRun classRun, List<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return
                course.HasContentCreatorPermission(userId, userRoles, haveFullRight) ||
                course.HasFacilitatorPermission(userId, userRoles, haveFullRight) ||
                course.HasAdministrationPermission(userId, userRoles, haveFullRight) ||
                classRun.HasFacilitatorPermission(userId, userRoles, haveFullRight(course));
        }

        public static Expression<Func<Registration, bool>> IsOfferPendingExpr()
        {
            return IsExistedExpr().AndAlso(x =>
               (x.Status == RegistrationStatus.OfferPendingApprovalByLearner && x.OfferSentDate != null));
        }

        public static Expression<Func<Registration, bool>> AtLeastPercentContentsCompletedExpr(double atleastPercentContents)
        {
            return x => x.LearningContentProgress.HasValue && x.LearningContentProgress >= atleastPercentContents;
        }

        public static Expression<Func<Registration, bool>> NeverLearningCompletedExpr()
        {
            return x => !x.LearningCompletedDate.HasValue;
        }

        public static Expression<Func<Registration, bool>> IsWithdrawalAdministratingExpr()
        {
            return x => x.WithdrawalStatus.HasValue && x.WithdrawalStatus != Enums.WithdrawalStatus.PendingConfirmation
                                                    && x.WithdrawalStatus != Enums.WithdrawalStatus.Rejected;
        }

        public static Expression<Func<Registration, bool>> IsPendingWithdrawalConfirmExpr()
        {
            return x => x.WithdrawalStatus == Enums.WithdrawalStatus.Approved;
        }

        public static Expression<Func<Registration, bool>> WaitingForParticipantSelectionExpr()
        {
            return x => x.Status == RegistrationStatus.Approved || x.Status == RegistrationStatus.WaitlistConfirmed;
        }

        public static ExpressionValidator<Registration> CanCreateWithdrawRequestValidator(CourseEntity course, ClassRun classRun)
        {
            return new ExpressionValidator<Registration>(
                x => (!x.WithdrawalStatus.HasValue ||
                       x.WithdrawalStatus == Enums.WithdrawalStatus.Rejected ||
                       x.WithdrawalStatus == Enums.WithdrawalStatus.RejectedByCA) &&
                      x.LearningStatus == LearningStatus.NotStarted &&
                      classRun.Published() &&
                      !classRun.Started() &&
                      course.IsNotArchived(),
                "Can only create withdrawn request for not pending withdraw approval, not learning started, published and not started classrun and not archived course.");
        }

        public static Expression<Func<Registration, bool>> IsExistedExpr()
        {
            return x => x.WithdrawalStatus != Enums.WithdrawalStatus.Withdrawn
                    && x.ClassRunChangeStatus != Enums.ClassRunChangeStatus.ConfirmedByCA
                    && x.IsExpired == false;
        }

        public static ExpressionValidator<Registration> CanProcessPendingApprovalByLearnerValidator(CourseEntity course, RegistrationStatus newStatus)
        {
            Expression<Func<Registration, bool>> expr1 = p => !course.IsArchived();
            expr1 = expr1.AndAlso(IsNotExpiredExpr());

            if (newStatus == RegistrationStatus.WaitlistConfirmed
                || newStatus == RegistrationStatus.WaitlistRejected)
            {
                return new ExpressionValidator<Registration>(
                    expr1.AndAlso(p => p.Status == RegistrationStatus.WaitlistPendingApprovalByLearner),
                    "Can only process for not expired, wait list pending approval registration and course is not archived.");
            }

            if (newStatus == RegistrationStatus.OfferConfirmed
                || newStatus == RegistrationStatus.OfferRejected)
            {
                return new ExpressionValidator<Registration>(
                    expr1.AndAlso(p => p.Status == RegistrationStatus.OfferPendingApprovalByLearner),
                    "Can only process for not expired,  offer pending approval registration and course is not archived.");
            }

            return new ExpressionValidator<Registration>(expr1, "Can only process for not expired registration and course is not archived.");
        }

        public static ExpressionValidator<Registration> CanBeApprovalValidator()
        {
            return new ExpressionValidator<Registration>(p => p.Status == RegistrationStatus.PendingConfirmation, "Can only approve/reject a pending confirmation registration.");
        }

        public static ExpressionValidator<Registration> CanApproveRejectWithdrawRequestValidator(CourseEntity course)
        {
            return new ExpressionValidator<Registration>(
                x => (
                      x.WithdrawalStatus == Enums.WithdrawalStatus.Approved
                      || x.Status == RegistrationStatus.Approved
                      || x.Status == RegistrationStatus.ConfirmedByCA
                      || x.Status == RegistrationStatus.OfferConfirmed
                      || x.Status == RegistrationStatus.WaitlistPendingApprovalByLearner
                      || x.Status == RegistrationStatus.WaitlistConfirmed
                      || x.Status == RegistrationStatus.OfferPendingApprovalByLearner)
                     && course.IsNotArchived()
                     && !x.IsExpired,
                "Can only approve/reject an approved by AO withdraw request or Approved/ConfirmedByCA/OfferConfirmed/WaitlistPendingApprovalByLearner/WaitlistConfirmed/OfferPendingApprovalByLearner registration, and not archived course.");
        }

        public static ExpressionValidator<Registration> CanApproveRejectChangeClassRunRequestValidator(CourseEntity course)
        {
            return new ExpressionValidator<Registration>(
                x => x.ClassRunChangeStatus == Enums.ClassRunChangeStatus.PendingConfirmation
                     && course.IsNotArchived()
                     && !x.IsExpired,
                "Can only approve/reject a pending confirmation change classrun request registration, and not archived course.");
        }

        public static ExpressionValidator<Registration> CanConfirmByCAValidator()
        {
            return new ExpressionValidator<Registration>(
                p => p.Status == RegistrationStatus.Approved && p.CanByPassCourseCriteriaViolation() && !p.IsExpired,
                "Can only approve/reject an approved, not expired, not violate course criteria registration.");
        }

        public static ExpressionValidator<Registration> CanRejectByCAValidator()
        {
            return new ExpressionValidator<Registration>(
                p => p.Status == RegistrationStatus.Approved && !p.IsExpired,
                "Can only approve/reject an approved, not expired registration.");
        }

        public static ExpressionValidator<Registration> CanApproveRejectWaitlistValidator()
        {
            return new ExpressionValidator<Registration>(
                p => p.Status == RegistrationStatus.WaitlistPendingApprovalByLearner && !p.IsExpired,
                "Can only approve/reject to wait list an wait list pending approval, not expired registration.");
        }

        public static ExpressionValidator<Registration> CanApproveRejectOfferValidator()
        {
            return new ExpressionValidator<Registration>(
                p => p.Status == RegistrationStatus.OfferPendingApprovalByLearner && !p.IsExpired,
                "Can only approve/reject an offer pending approval, not expired registration.");
        }

        public static ExpressionValidator<Registration> CanMoveToWaitlistManuallyValidator()
        {
            return new ExpressionValidator<Registration>(
                p => p.Status == RegistrationStatus.Approved && p.CanByPassCourseCriteriaViolation() && !p.IsExpired,
                "Can only move to wait list an approved, not violate course criteria, not expired registration.");
        }

        public static ExpressionValidator<Registration> CanOfferLearnerManuallyValidator()
        {
            return new ExpressionValidator<Registration>(
                p => p.Status == RegistrationStatus.WaitlistConfirmed && p.CanByPassCourseCriteriaViolation() && !p.IsExpired,
                "Can only offer learner an in wait list, not violate course criteria, not expired registration.");
        }

        public static ExpressionValidator<Registration> CanSetOfferExpiredValidator()
        {
            return new ExpressionValidator<Registration>(
                p => p.IsOfferPending() && !p.IsExpired,
                "Can only set offer expired an pending offer, not expired registration.");
        }

        public bool CanCompletePostCourseEvaluation(CourseEntity course)
        {
            return CanCompletePostCourseEvaluationExpr(course).Compile()(this);
        }

        public bool IsPending()
        {
            return IsPendingExpr().Compile()(this);
        }

        public bool IsOfferPending()
        {
            return IsOfferPendingExpr().Compile()(this);
        }

        public bool InProgress()
        {
            return InProgressExpr().Compile()(this);
        }

        public bool IsRejected()
        {
            return IsRejectedExpr().Compile()(this);
        }

        public bool IsCompleted()
        {
            return IsCompletedExpr().Compile()(this);
        }

        public bool IsFailed()
        {
            return IsFailedExpr().Compile()(this);
        }

        public bool IsLearningFinished()
        {
            return IsLearningFinishedExpr().Compile()(this);
        }

        public bool IsNotAbleToBeNominated()
        {
            return IsNotAbleToBeNominatedExpr().Compile()(this);
        }

        public bool AtleastPercentContentsCompleted(double atleastPercentContent)
        {
            return AtLeastPercentContentsCompletedExpr(atleastPercentContent).Compile()(this);
        }

        public bool CanChangeClassRun(CourseEntity course)
        {
            return CanChangeClassRunExpr(course).Compile()(this);
        }

        public Validation<Registration> ValidateCanChangeClassRun(CourseEntity course)
        {
            return CanChangeClassRunValidator(course).Validate(this);
        }

        public bool HasChangeClassRunPermission(CourseEntity course, Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasChangeClassRunPermissionExpr(course, userId, userRoles, haveFullRight).Compile()(this);
        }

        public Validation ValidateCanCreateWithdrawRequest(CourseEntity course, ClassRun classRun)
        {
            return CanCreateWithdrawRequestValidator(course, classRun).Validate(this);
        }

        public Validation<Registration> ValidateCanBeApproval()
        {
            return CanBeApprovalValidator().Validate(this);
        }

        public Validation<Registration> ValidateCanApproveRejectWithdrawRequest(CourseEntity course)
        {
            return CanApproveRejectWithdrawRequestValidator(course).Validate(this);
        }

        public Validation<Registration> ValidateCanApproveRejectChangeClassRunRequest(CourseEntity course)
        {
            return CanApproveRejectChangeClassRunRequestValidator(course).Validate(this);
        }

        public Validation<Registration> ValidateCanConfirmByCA()
        {
            return CanConfirmByCAValidator().Validate(this);
        }

        public Validation<Registration> ValidateCanRejectByCA()
        {
            return CanRejectByCAValidator().Validate(this);
        }

        public Validation<Registration> ValidateCanApproveRejectWaitlist()
        {
            return CanApproveRejectWaitlistValidator().Validate(this);
        }

        public Validation<Registration> ValidateCanApproveRejectOffer()
        {
            return CanApproveRejectOfferValidator().Validate(this);
        }

        public Validation<Registration> ValidateCanMoveToWaitlistManually()
        {
            return CanMoveToWaitlistManuallyValidator().Validate(this);
        }

        public Validation<Registration> ValidateCanOfferLearnerManually()
        {
            return CanOfferLearnerManuallyValidator().Validate(this);
        }

        public Validation<Registration> ValidateCanSetOfferExpired()
        {
            return CanSetOfferExpiredValidator().Validate(this);
        }

        public bool HasManagePermission(
            CourseEntity course,
            ClassRun classRun,
            Guid? userId,
            List<string> userRoles,
            Func<CourseEntity, bool> haveCourseFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return HasManagePermissionExpr(course, classRun, userId, userRoles, haveCourseFullRight, checkHasPermissionFn).Compile()(this);
        }

        public bool HasCreateWithdrawRequestPermission(
            Guid? userId,
            List<string> userRoles)
        {
            return HasCreateWithdrawRequestPermissionExpr(userId, userRoles).Compile()(this);
        }

        public bool HasApproveRejectWaitlistPermission(
            Guid? userId,
            List<string> userRoles)
        {
            return HasApproveRejectWaitlistPermissionExpr(userId, userRoles).Compile()(this);
        }

        public bool HasApproveRejectOfferPermission(
            Guid? userId,
            List<string> userRoles)
        {
            return HasApproveRejectOfferPermissionExpr(userId, userRoles).Compile()(this);
        }

        public bool NeverLearningCompleted()
        {
            return NeverLearningCompletedExpr().Compile()(this);
        }

        public bool IsSlotTaking()
        {
            return IsSlotTakingExpr().Compile()(this);
        }

        public bool CanByPassCourseCriteriaViolation()
        {
            return CourseCriteriaViolated != true || CourseCriteriaOverrided == true;
        }

        public bool CanPassCourseCriteriaCheckForClassRun(ClassRun classrun)
        {
            return !classrun.CourseCriteriaActivated || (CourseCriteriaViolation != null && !CourseCriteriaViolation.IsViolated);
        }

        public bool WaitingForParticipantSelection()
        {
            return WaitingForParticipantSelectionExpr().Compile()(this);
        }

        public bool CanTriggerPostCourseEvaluation()
        {
            return CanTriggerPostCourseEvaluationExpr().Compile()(this);
        }

        public Validation ValidateCanCompletePostCourseEvaluation(ClassRun classRun, CourseEntity course)
        {
            return Validation.ValidIf(
                (classRun.Ended() ||
                 LearningStatus == LearningStatus.Passed ||
                 LearningStatus == LearningStatus.Failed) &&
                 !PostCourseEvaluationFormCompleted &&
                course.IsNotArchived(),
                "Cannot complete post course evaluation for this registration");
        }

        public Validation ValidateNotExpired()
        {
            return Validation.ValidIf(!this.IsExpired, "Cannot action on expired registration");
        }

        public Validation ValidateCanProcessPendingApprovalByLearner(CourseEntity course, RegistrationStatus newStatus)
        {
            return CanProcessPendingApprovalByLearnerValidator(course, newStatus).Validate(this);
        }

        public Validation ValidateCanUpdateLearningStatus(LearningStatus learningStatus)
        {
            if (LearningStatus == learningStatus)
            {
                return Validation.Invalid($"Current course learning status is already {learningStatus}");
            }

            if (learningStatus == LearningStatus.InProgress)
            {
                return Validation.Invalid($"Current course learning status must be {LearningStatus.NotStarted}");
            }

            return Validation.Valid();
        }

        public Validation ValidateCanBeRegisterNewForNotAddedByCA(
            CourseEntity course,
            ClassRun classRun,
            List<Registration> existedSameUserAndCourseInProgressRegistrations)
        {
            if (course.OnlyForNominatedRegistration() && RegistrationType != RegistrationType.Nominated)
            {
                return Validation.Invalid("Unable to apply this course because you haven’t finished the prerequisite course(s)");
            }

            var canApplyIntoClassRunValidation = classRun.ValidateCanApplyIntoClassRun(course);
            if (!canApplyIntoClassRunValidation.IsValid)
            {
                return canApplyIntoClassRunValidation;
            }

            if (existedSameUserAndCourseInProgressRegistrations.Count >= course.MaxReLearningTimes)
            {
                return Validation.Invalid("You have exceeded max learning times of the course");
            }

            if (existedSameUserAndCourseInProgressRegistrations.Any(p => !p.IsLearningFinished()))
            {
                return Validation.Invalid("You have not finished learning this course");
            }

            return Validation.Valid();
        }

        public void UpdateLearningStatus(LearningStatus newStatus)
        {
            LearningCompletedDate = newStatus == LearningStatus.Completed
                ? Clock.Now
                : LearningCompletedDate;

            LearningStatus = newStatus;
        }
    }
}
