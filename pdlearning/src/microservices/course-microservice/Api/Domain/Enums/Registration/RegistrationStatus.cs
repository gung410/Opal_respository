namespace Microservice.Course.Domain.Enums
{
    public enum RegistrationStatus
    {
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
        AddedByCAConflict,
        AddedByCAClassfull,
        OfferExpired
    }
}
