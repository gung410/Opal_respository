namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedCourseInPlanningCycleNotifyOwnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string PlanningCyclePeriod { get; set; }

        public string CPCName { get; set; }

        public string CPCComment { get; set; }
    }
}
