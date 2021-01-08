namespace LearnerApp.Common
{
    public enum RegistrationStatus
    {
        Approved,
        ConfirmedByCA,
        OfferConfirmed,
        OfferPendingApprovalByLearner,
        OfferRejected,
        PendingConfirmation,
        Rejected,
        RejectedByCA,
        WaitlistConfirmed,
        WaitlistPendingApprovalByLearner,
        WaitlistRejected,
        WaitlistUnsuccessful,
        Cancelled,
        Rescheduled,
        ConfirmedBeforeStartDate,
        AddedByCAConflict,
        AddedByCAClassfull
    }
}
