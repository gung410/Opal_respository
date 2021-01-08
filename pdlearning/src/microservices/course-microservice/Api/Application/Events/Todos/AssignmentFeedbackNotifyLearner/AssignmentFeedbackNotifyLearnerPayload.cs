namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentFeedbackNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassRunTitle { get; set; }
    }
}
