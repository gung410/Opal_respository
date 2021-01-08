namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerRegistrationRejectedNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string ClassrunTitle { get; set; }

        public string CourseTitle { get; set; }
    }
}
