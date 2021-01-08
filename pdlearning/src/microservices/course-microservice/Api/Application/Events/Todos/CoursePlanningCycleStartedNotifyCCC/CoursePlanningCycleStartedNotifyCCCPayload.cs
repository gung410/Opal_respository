namespace Microservice.Course.Application.Events.Todos
{
    public class CoursePlanningCycleStartedNotifyCCCPayload : BaseTodoEventPayload
    {
        public string CoursePlanningCyclePeriod { get; set; }

        public string PlanningStartDate { get; set; }

        public string PlanningEndDate { get; set; }

        public string CPCName { get; set; }

        public string CPCEmail { get; set; }
    }
}
