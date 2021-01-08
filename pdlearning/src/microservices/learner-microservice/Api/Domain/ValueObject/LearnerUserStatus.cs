namespace Microservice.Learner.Domain.ValueObject
{
    public enum LearnerUserStatus
    {
        Unknown,
        Active,
        Inactive,
        Deactive,
        Pending,
        PendingApproval1st,
        PendingApproval2nd,
        PendingApproval3rd,
        New,
        IdentityServerLocked,
        Hidden,
        Rejected
    }
}
