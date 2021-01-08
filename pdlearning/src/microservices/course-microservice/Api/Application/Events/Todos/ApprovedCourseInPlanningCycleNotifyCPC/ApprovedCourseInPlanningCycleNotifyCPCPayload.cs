namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseInPlanningCycleNotifyCPCPayload : BaseTodoEventPayload
    {
        public string OwnerName { get; set; }

        public string PlanningCyclePeriod { get; set; }
    }
}
