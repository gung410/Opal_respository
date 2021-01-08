namespace Microservice.Course.Domain.Enums
{
    public enum CourseUserStatus
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
