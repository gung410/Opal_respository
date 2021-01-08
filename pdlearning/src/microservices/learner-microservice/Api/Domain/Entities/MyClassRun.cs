using System;
using System.Linq.Expressions;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Enums = Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from Registration table on the CAM module.
    /// </summary>
    public class MyClassRun : FullAuditedEntity, ISoftDelete
    {
        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public RegistrationStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public WithdrawalStatus? WithdrawalStatus { get; set; }

        public Guid RegistrationId { get; set; }

        public RegistrationType RegistrationType { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? AdministratedBy { get; set; }

        public ClassRunChangeStatus? ClassRunChangeStatus { get; set; }

        public DateTime? ClassRunChangeRequestedDate { get; set; }

        public Guid? ClassRunChangeId { get; set; }

        public LearningStatus? LearningStatus { get; set; }

        public bool PostCourseEvaluationFormCompleted { get; set; }

        public double? LearningContentProgress { get; set; }

        public bool IsExpired { get; set; }

        public static Expression<Func<MyClassRun, bool>> FilterInProgressExpr()
        {
            return p =>
                (p.Status == RegistrationStatus.Approved
                 || p.Status == RegistrationStatus.ConfirmedByCA
                 || p.Status == RegistrationStatus.OfferConfirmed
                 || p.Status == RegistrationStatus.WaitlistConfirmed
                 || p.Status == RegistrationStatus.PendingConfirmation
                 || p.Status == RegistrationStatus.OfferPendingApprovalByLearner
                 || p.Status == RegistrationStatus.WaitlistPendingApprovalByLearner)
                && p.WithdrawalStatus != Enums.WithdrawalStatus.Withdrawn
                && p.IsExpired == false;
        }

        public static Expression<Func<MyClassRun, bool>> FilterPendingExpr()
        {
            return p =>
                (p.Status == RegistrationStatus.Approved
                 || p.Status == RegistrationStatus.WaitlistConfirmed
                 || p.Status == RegistrationStatus.PendingConfirmation
                 || p.Status == RegistrationStatus.OfferPendingApprovalByLearner
                 || p.Status == RegistrationStatus.WaitlistPendingApprovalByLearner)
                && p.WithdrawalStatus != Enums.WithdrawalStatus.Withdrawn
                && p.IsExpired == false;
        }

        public static Expression<Func<MyClassRun, bool>> FilterClassChangeConfirmedExpr()
        {
            return p => p.ClassRunChangeStatus == Enums.ClassRunChangeStatus.ConfirmedByCA;
        }

        public static Expression<Func<MyClassRun, bool>> FilterPendingClassChangeExpr()
        {
            return p => p.ClassRunChangeStatus != Enums.ClassRunChangeStatus.ConfirmedByCA;
        }

        public static Expression<Func<MyClassRun, bool>> FilterFinishedLearningExpr()
        {
            return p =>
                p.LearningStatus == Enums.LearningStatus.Completed
                || p.LearningStatus == Enums.LearningStatus.Failed;
        }

        public static Expression<Func<MyClassRun, bool>> FilterParticipantExpr()
        {
            return p =>
                (p.Status == RegistrationStatus.ConfirmedByCA
                || p.Status == RegistrationStatus.OfferConfirmed)
                && p.WithdrawalStatus != Enums.WithdrawalStatus.Withdrawn;
        }

        /// <summary>
        /// The registration is in-progress.
        /// </summary>
        /// <returns>Returns true if the status is
        /// <see cref="RegistrationStatus.Approved"/>
        /// or <see cref="RegistrationStatus.ConfirmedByCA"/>
        /// or <see cref="RegistrationStatus.OfferConfirmed"/>
        /// or <see cref="RegistrationStatus.PendingConfirmation"/>
        /// or <see cref="RegistrationStatus.OfferPendingApprovalByLearner"/>
        /// or <see cref="RegistrationStatus.WaitlistPendingApprovalByLearner"/>
        /// and the WithdrawalStatus is not equal to <see cref="Enums.WithdrawalStatus.Withdrawn"/>,
        /// otherwise false.</returns>
        public bool IsInProgress()
        {
            return FilterInProgressExpr().Compile()(this);
        }

        /// <summary>
        /// To check the learning has finished or not.
        /// </summary>
        /// <returns>Returns true if the learning status is Completed or Failed.</returns>
        public bool IsFinishedLearning()
        {
            return FilterFinishedLearningExpr().Compile()(this);
        }

        /// <summary>
        /// To check the registration status has changed or not.
        /// </summary>
        /// <param name="registrationStatus">
        /// Registration status as value object <see cref="ValueObject.RegistrationStatus"/>.</param>
        /// <returns>Returns true if the current Status is not equal to the Status from the registration message,
        /// otherwise false.</returns>
        public bool HasRegistrationStatusChanged(RegistrationStatus registrationStatus)
        {
            return Status != registrationStatus;
        }

        /// <summary>
        /// To check the withdraw status has changed or not.
        /// </summary>
        /// <param name="withdrawalStatus">
        /// WithdrawalStatus as value object <see cref="ValueObject.WithdrawalStatus"/>.</param>
        /// <returns>Return true if WithdrawalStatus is not equal to the WithdrawalStatus
        /// from the registration message, otherwise false.</returns>
        public bool HasWithdrawStatusChanged(WithdrawalStatus? withdrawalStatus)
        {
            return WithdrawalStatus != withdrawalStatus;
        }

        /// <summary>
        /// To check the change class run status has changed or not.
        /// </summary>
        /// <param name="classRunChangeStatus">
        /// Class run change status as value object <see cref="ClassRunChangeStatus"/>.</param>
        /// <returns>Returns true if the current ClassRunChangeStatus is not equal to the ClassRunChangeStatus
        /// from the registration message, otherwise false.</returns>
        public bool HasChangeClassStatusChanged(ClassRunChangeStatus? classRunChangeStatus)
        {
            return ClassRunChangeStatus != classRunChangeStatus;
        }

        /// <summary>
        /// To check the registration type has changed or not.
        /// </summary>
        /// <param name="registrationType">RegistrationType value object.</param>
        /// <returns>Returns true if the current RegistrationType is not equal to the RegistrationType
        /// from the registration message, otherwise false.</returns>
        public bool HasRegistrationTypeChanged(RegistrationType registrationType)
        {
            return RegistrationType != registrationType;
        }

        /// <summary>
        /// To check class run change status has been approved or not.
        /// </summary>
        /// <returns>
        /// Returns true if ClassRunChangeStatus is equal to <see cref="Enums.ClassRunChangeStatus.Approved"/>,
        /// otherwise false.</returns>
        public bool IsApprovedClassChange()
        {
            return ClassRunChangeStatus == Enums.ClassRunChangeStatus.Approved;
        }

        /// <summary>
        /// To check pending registration from course administrator or approving officer or learner or not.
        /// </summary>
        /// <returns>Returns true if the Status is
        /// <see cref="RegistrationStatus.Approved"/>
        /// or <see cref="RegistrationStatus.WaitlistConfirmed"/>
        /// or <see cref="RegistrationStatus.PendingConfirmation"/>
        /// or <see cref="RegistrationStatus.OfferPendingApprovalByLearner"/>
        /// or <see cref="RegistrationStatus.WaitlistPendingApprovalByLearner"/>
        /// and WithdrawalStatus is not equal to <see cref="Enums.WithdrawalStatus.Withdrawn"/>,
        /// otherwise false.</returns>
        public bool IsPending()
        {
            return FilterPendingExpr().Compile()(this);
        }

        public bool IsClassFullOrConflict()
        {
            return Status == RegistrationStatus.AddedByCAConflict
                   || Status == RegistrationStatus.AddedByCAClassfull;
        }
    }
}
