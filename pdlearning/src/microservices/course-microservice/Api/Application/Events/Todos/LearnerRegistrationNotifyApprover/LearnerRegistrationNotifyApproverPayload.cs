namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerRegistrationNotifyApproverPayload : BaseTodoEventPayload
    {
        public string LearnerName { get; set; }

        public string LearnerEmail { get; set; }

        public string ClassrunTitle { get; set; }

        public string CourseTitle { get; set; }
    }
}
