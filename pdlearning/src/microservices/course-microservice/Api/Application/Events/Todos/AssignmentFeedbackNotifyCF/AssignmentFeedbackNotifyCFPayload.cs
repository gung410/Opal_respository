namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentFeedbackNotifyCFPayload : BaseTodoEventPayload
    {
        public string LearnerName { get; set; }

        public string ClassrunTitle { get; set; }

        public string CourseTitle { get; set; }
    }
}
