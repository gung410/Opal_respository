using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Common
{
    /// <summary>
    /// A mapper to map from business status to Display Status.
    /// </summary>
    public static class DisplayStatusMapper
    {
        private static readonly IReadOnlyDictionary<WithdrawalStatus, DisplayStatus> WithdrawalStatusMap
            = new Dictionary<WithdrawalStatus, DisplayStatus>
            {
                { WithdrawalStatus.PendingConfirmation, DisplayStatus.WithdrawalPendingConfirmation },
                { WithdrawalStatus.Rejected, DisplayStatus.WithdrawalRejected },
                { WithdrawalStatus.Approved, DisplayStatus.WithdrawalApproved },
                { WithdrawalStatus.Withdrawn, DisplayStatus.WithdrawalWithdrawn },
                { WithdrawalStatus.RejectedByCA, DisplayStatus.WithdrawalRejectedByCA }
            };

        private static readonly IReadOnlyDictionary<ClassRunChangeStatus, DisplayStatus> ClassRunChangeStatusMap
            = new Dictionary<ClassRunChangeStatus, DisplayStatus>
            {
                { ClassRunChangeStatus.PendingConfirmation, DisplayStatus.ClassRunChangePendingConfirmation },
                { ClassRunChangeStatus.Rejected, DisplayStatus.ClassRunChangeRejected },
                { ClassRunChangeStatus.Approved, DisplayStatus.ClassRunChangeApproved },
                { ClassRunChangeStatus.ConfirmedByCA, DisplayStatus.ClassRunChangeConfirmedByCA },
                { ClassRunChangeStatus.RejectedByCA, DisplayStatus.ClassRunChangeRejectedByCA }
            };

        private static readonly IReadOnlyDictionary<RegistrationStatus, DisplayStatus> RegistrationStatusMap
            = new Dictionary<RegistrationStatus, DisplayStatus>
            {
                { RegistrationStatus.PendingConfirmation, DisplayStatus.PendingConfirmation },
                { RegistrationStatus.Approved, DisplayStatus.Approved },
                { RegistrationStatus.Rejected, DisplayStatus.Rejected },
                { RegistrationStatus.ConfirmedByCA, DisplayStatus.ConfirmedByCA },
                { RegistrationStatus.RejectedByCA, DisplayStatus.RejectedByCA },
                { RegistrationStatus.WaitlistPendingApprovalByLearner, DisplayStatus.WaitlistPendingApprovalByLearner },
                { RegistrationStatus.WaitlistConfirmed, DisplayStatus.WaitlistConfirmed },
                { RegistrationStatus.WaitlistRejected, DisplayStatus.WaitlistRejected },
                { RegistrationStatus.OfferPendingApprovalByLearner, DisplayStatus.OfferPendingApprovalByLearner },
                { RegistrationStatus.OfferRejected, DisplayStatus.OfferRejected },
                { RegistrationStatus.OfferConfirmed, DisplayStatus.OfferConfirmed }
            };

        private static readonly IReadOnlyDictionary<RegistrationStatus, DisplayStatus> NominatedStatusMap
            = new Dictionary<RegistrationStatus, DisplayStatus>
            {
                { RegistrationStatus.PendingConfirmation, DisplayStatus.NominatedPendingConfirmation },
                { RegistrationStatus.Approved, DisplayStatus.NominatedApproved },
                { RegistrationStatus.Rejected, DisplayStatus.NominatedRejected },
                { RegistrationStatus.ConfirmedByCA, DisplayStatus.NominatedConfirmedByCA },
                { RegistrationStatus.RejectedByCA, DisplayStatus.NominatedRejectedByCA },
                { RegistrationStatus.WaitlistPendingApprovalByLearner, DisplayStatus.NominatedWaitlistPendingApprovalByLearner },
                { RegistrationStatus.WaitlistConfirmed, DisplayStatus.NominatedWaitlistConfirmed },
                { RegistrationStatus.WaitlistRejected, DisplayStatus.NominatedWaitlistRejected },
                { RegistrationStatus.OfferPendingApprovalByLearner, DisplayStatus.NominatedOfferPendingApprovalByLearner },
                { RegistrationStatus.OfferRejected, DisplayStatus.NominatedOfferRejected },
                { RegistrationStatus.OfferConfirmed, DisplayStatus.NominatedOfferConfirmed }
            };

        private static readonly IReadOnlyDictionary<RegistrationStatus, DisplayStatus> AddedByCAStatusMap
            = new Dictionary<RegistrationStatus, DisplayStatus>
            {
                { RegistrationStatus.PendingConfirmation, DisplayStatus.AddedByCAPendingConfirmation },
                { RegistrationStatus.Approved, DisplayStatus.AddedByCAApproved },
                { RegistrationStatus.Rejected, DisplayStatus.AddedByCARejected },
                { RegistrationStatus.ConfirmedByCA, DisplayStatus.AddedByCAConfirmedByCA },
                { RegistrationStatus.RejectedByCA, DisplayStatus.AddedByCARejectedByCA },
                { RegistrationStatus.WaitlistPendingApprovalByLearner, DisplayStatus.AddedByCAWaitlistPendingApprovalByLearner },
                { RegistrationStatus.WaitlistConfirmed, DisplayStatus.AddedByCAWaitlistConfirmed },
                { RegistrationStatus.WaitlistRejected, DisplayStatus.AddedByCAWaitlistRejected },
                { RegistrationStatus.OfferPendingApprovalByLearner, DisplayStatus.AddedByCAOfferPendingApprovalByLearner },
                { RegistrationStatus.OfferRejected, DisplayStatus.AddedByCAOfferRejected },
                { RegistrationStatus.OfferConfirmed, DisplayStatus.AddedByCAOfferConfirmed }
            };

        private static readonly IReadOnlyDictionary<RegistrationType, DisplayStatus> CancelledClassRunMap
            = new Dictionary<RegistrationType, DisplayStatus>
            {
                { RegistrationType.Manual, DisplayStatus.Cancelled },
                { RegistrationType.Nominated, DisplayStatus.NominatedCancelled },
                { RegistrationType.AddedByCA, DisplayStatus.AddedByCACancelled }
            };

        private static readonly IReadOnlyDictionary<RegistrationType, DisplayStatus> RescheduledClassRunMap
            = new Dictionary<RegistrationType, DisplayStatus>
            {
                { RegistrationType.Manual, DisplayStatus.Rescheduled },
                { RegistrationType.Nominated, DisplayStatus.NominatedRescheduled },
                { RegistrationType.AddedByCA, DisplayStatus.AddedByCARescheduled }
            };

        public static DisplayStatus MapFromWithdrawalStatus(WithdrawalStatus status)
        {
            return WithdrawalStatusMap[status];
        }

        public static DisplayStatus MapFromClassRunChangeStatus(ClassRunChangeStatus status)
        {
            return ClassRunChangeStatusMap[status];
        }

        public static DisplayStatus MapFromRegistrationStatus(RegistrationStatus status)
        {
            return RegistrationStatusMap[status];
        }

        public static DisplayStatus MapFromNominatedStatus(RegistrationStatus status)
        {
            return NominatedStatusMap[status];
        }

        public static DisplayStatus MapFromAddedByCAStatus(RegistrationStatus status)
        {
            return AddedByCAStatusMap[status];
        }

        public static DisplayStatus MapFromClassRunCancelled(RegistrationType type)
        {
            return CancelledClassRunMap[type];
        }

        public static DisplayStatus MapFromClassRunRescheduled(RegistrationType type)
        {
            return RescheduledClassRunMap[type];
        }
    }
}
