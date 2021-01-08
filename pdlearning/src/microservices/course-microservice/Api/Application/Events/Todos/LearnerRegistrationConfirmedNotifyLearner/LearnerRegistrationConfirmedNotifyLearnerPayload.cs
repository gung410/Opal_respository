namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerRegistrationConfirmedNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassrunTitle { get; set; }
    }
}
