namespace Microservice.Learner.Domain.ValueObject
{
    public enum ClassRunChangeStatus
    {
        PendingConfirmation,
        Rejected,
        Approved,
        ConfirmedByCA,
        RejectedByCA,
    }
}
