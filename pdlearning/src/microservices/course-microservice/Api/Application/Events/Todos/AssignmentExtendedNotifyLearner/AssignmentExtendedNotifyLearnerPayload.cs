namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentExtendedNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassRunTitle { get; set; }

        public string ExtendedDueDate { get; set; }
    }
}
