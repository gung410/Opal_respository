namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerRegistrationNotifyAdministratorPayload : BaseTodoEventPayload
    {
        public string ClassrunTitle { get; set; }

        public string CourseTitle { get; set; }
    }
}
