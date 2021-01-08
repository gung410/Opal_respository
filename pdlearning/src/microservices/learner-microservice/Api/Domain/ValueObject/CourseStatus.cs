namespace Microservice.Learner.Domain.ValueObject
{
    public enum CourseStatus
    {
        Draft,
        PendingApproval,
        Rejected,
        Approved,
        Published,
        Unpublished,
        PlanningCycleVerified,
        PlanningCycleCompleted,
        VerificationRejected,
        Completed,
        Archived
    }
}
