namespace Microservice.Learner.Domain.ValueObject
{
    public enum RegistrationType
    {
        /// <summary>
        /// EF core cannot insert enum start from first item. Then we must be added None.
        /// </summary>
        None,
        Manual,
        Application,
        Nominated,
        AddedByCA
    }

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
        AddedByCAClassfull
    }
}
