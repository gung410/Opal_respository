using System;
using Microservice.Learner.Domain.ValueObject;
using ValueObject = Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
    public class RegistrationChangeMessage
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public RegistrationStatus Status { get; set; }

        public LearningStatus? LearningStatus { get; set; }

        public WithdrawalStatus? WithdrawalStatus { get; set; }

        public DateTime? ChangedDate { get; set; }

        public RegistrationType RegistrationType { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? AdministratedBy { get; set; }

        public ClassRunChangeStatus? ClassRunChangeStatus { get; set; }

        public DateTime? ClassRunChangeRequestedDate { get; set; }

        public Guid? ClassRunChangeId { get; set; }

        public bool PostCourseEvaluationFormCompleted { get; set; }

        public DateTime? LearningCompletedDate { get; set; }

        public double? LearningContentProgress { get; set; }

        public bool IsExpired { get; set; }

        /// <summary>
        /// <para>Rejected status is performed by the approving officer.</para>
        /// <para>RejectedByCA is performed by the course administrator.</para>
        /// <para>WaitListRejected and OfferRejected is performed by Learner.</para>
        /// </summary>
        /// <returns>Returns true if rejected statuses, otherwise false.</returns>
        public bool IsRejectStatuses()
        {
            return Status == RegistrationStatus.Rejected ||
                   Status == RegistrationStatus.RejectedByCA ||
                   Status == RegistrationStatus.WaitlistRejected ||
                   Status == RegistrationStatus.OfferRejected;
        }

        /// <summary>
        /// <para>WaitListPendingApprovalByLearner and OfferPendingApprovalByLearner status is performed by the course administrator.</para>
        /// <para>WaitListConfirmed and OfferConfirmed is performed by Learner.</para>
        /// </summary>
        /// <returns>Returns true if the wait list status is WaitlistPendingApprovalByLearner or WaitlistConfirmed or OfferPendingApprovalByLearner or OfferConfirmed,
        /// otherwise false.</returns>
        public bool IsWaitListStatuses()
        {
            return Status == RegistrationStatus.WaitlistPendingApprovalByLearner ||
                   Status == RegistrationStatus.WaitlistConfirmed ||
                   Status == RegistrationStatus.OfferPendingApprovalByLearner ||
                   Status == RegistrationStatus.OfferConfirmed;
        }

        /// <summary>
        /// <para>ConfirmedByCA is performed by the course administrator.</para>
        /// </summary>
        /// <returns>Returns true if registration status is ConfirmedByCA,
        /// otherwise false.</returns>
        public bool IsConfirmedByCourseAdministrator()
        {
            return Status == RegistrationStatus.ConfirmedByCA;
        }

        /// <summary>
        /// If the class is full or conflicting, user wont display on these status.
        /// It's only shown in the course module.
        /// </summary>
        /// <returns>Returns true if the class is full or conflicted,
        /// otherwise false.</returns>
        public bool IsClassFullOrConflict()
        {
            return Status == RegistrationStatus.AddedByCAConflict
                   || Status == RegistrationStatus.AddedByCAClassfull;
        }

        /// <summary>
        /// The user's learning has been finished.
        /// </summary>
        /// <returns>Returns true if the learning status is
        /// <see cref="ValueObject.LearningStatus.Failed"/>
        /// or <see cref="ValueObject.LearningStatus.Passed"/>
        /// or <see cref="ValueObject.LearningStatus.Completed"/>
        /// otherwise false.</returns>
        public bool IsFinishedLearning()
        {
            return LearningStatus == ValueObject.LearningStatus.Failed
                   || LearningStatus == ValueObject.LearningStatus.Passed
                   || LearningStatus == ValueObject.LearningStatus.Completed;
        }

        /// <summary>
        /// The class change request has been confirmed by course administrator.
        /// </summary>
        /// <returns>Returns true if the class run change status is ConfirmedByCA,
        /// otherwise false.</returns>
        public bool IsConfirmedClassChange()
        {
            return ClassRunChangeStatus == ValueObject.ClassRunChangeStatus.ConfirmedByCA;
        }

        /// <summary>
        /// The registration is created by the learner.
        /// </summary>
        /// <returns>Returns true if the registration type is manual,
        /// otherwise false.</returns>
        public bool IsManualRegistration()
        {
            return RegistrationType == RegistrationType.Manual;
        }

        /// <summary>
        /// The user's learning has completed.
        /// </summary>
        /// <returns>Returns true if the learning status is
        /// <see cref="ValueObject.LearningStatus.Completed"/>
        /// otherwise false.</returns>
        public bool IsLearningCompleted()
        {
            return LearningCompletedDate.HasValue && LearningStatus == ValueObject.LearningStatus.Completed;
        }

        /// <summary>
        /// Conditions for adding <see cref="OutstandingTaskType.Course"/> task.
        /// </summary>
        /// <returns>Returns true if the registration status is ConfirmedByCA or OfferConfirmed,
        /// the class change status is not ConfirmedByCA
        /// and the withdrawal status is not Withdrawn,
        /// otherwise false.</returns>
        public bool CanInsertOutstandingTask()
        {
            return IsParticipant() && !IsConfirmedClassChange() && !IsWithdrawnRegistration();
        }

        /// <summary>
        /// Conditions for deleting <see cref="OutstandingTaskType.Course"/> task.
        /// </summary>
        /// <returns>Returns true if the registration status is ConfirmedByCA or OfferConfirmed,
        /// the class change status is not ConfirmedByCA
        /// and the withdrawal status is not Withdrawn
        /// and the learning status is Failed or Completed,
        /// otherwise false.</returns>
        public bool CanDeleteOutstandingTask()
        {
            return LearningStatus == ValueObject.LearningStatus.Failed
                   || LearningStatus == ValueObject.LearningStatus.Completed
                   || (IsParticipant() && IsConfirmedClassChange() && IsWithdrawnRegistration());
        }

        /// <summary>
        /// The user has joined the class.
        /// </summary>
        /// <returns>Returns true if registration status is OfferConfirmed or ConfirmedByCA.</returns>
        private bool IsParticipant()
        {
            return Status == RegistrationStatus.OfferConfirmed || Status == RegistrationStatus.ConfirmedByCA;
        }

        /// <summary>
        /// The user's registration has been withdrawn.
        /// </summary>
        /// <returns>Returns true if the withdrawal status is Withdrawn, otherwise false.</returns>
        private bool IsWithdrawnRegistration()
        {
            return WithdrawalStatus == ValueObject.WithdrawalStatus.Withdrawn;
        }
    }
}
