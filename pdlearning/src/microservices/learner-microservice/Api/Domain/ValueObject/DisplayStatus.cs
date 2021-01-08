namespace Microservice.Learner.Domain.ValueObject
{
    public enum DisplayStatus
    {
        WithdrawalPendingConfirmation,
        WithdrawalRejected,
        WithdrawalApproved,
        WithdrawalWithdrawn,
        WithdrawalRejectedByCA,

        ClassRunChangePendingConfirmation,
        ClassRunChangeApproved,
        ClassRunChangeRejected,
        ClassRunChangeConfirmedByCA,
        ClassRunChangeRejectedByCA,

        PendingConfirmation,
        Approved,
        Rejected,
        ConfirmedByCA,
        RejectedByCA,
        WaitlistPendingApprovalByLearner,
        WaitlistConfirmed,
        WaitlistRejected,
        OfferPendingApprovalByLearner,
        OfferRejected,
        OfferConfirmed,
        Cancelled,
        Rescheduled,

        NominatedPendingConfirmation,
        NominatedApproved,
        NominatedRejected,
        NominatedConfirmedByCA,
        NominatedRejectedByCA,
        NominatedWaitlistPendingApprovalByLearner,
        NominatedWaitlistConfirmed,
        NominatedWaitlistRejected,
        NominatedOfferPendingApprovalByLearner,
        NominatedOfferRejected,
        NominatedOfferConfirmed,
        NominatedCancelled,
        NominatedRescheduled,

        AddedByCAPendingConfirmation,
        AddedByCAApproved,
        AddedByCARejected,
        AddedByCAConfirmedByCA,
        AddedByCARejectedByCA,
        AddedByCAWaitlistPendingApprovalByLearner,
        AddedByCAWaitlistConfirmed,
        AddedByCAWaitlistRejected,
        AddedByCAOfferPendingApprovalByLearner,
        AddedByCAOfferRejected,
        AddedByCAOfferConfirmed,
        AddedByCACancelled,
        AddedByCARescheduled
    }
}
