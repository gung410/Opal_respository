namespace Microservice.Course.Application.Events.Todos
{
    public class AssignedAssignmentNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string AssignmentName { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
