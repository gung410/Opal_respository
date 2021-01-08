namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerWaitlistNotifyAdministratorPayload : BaseTodoEventPayload
    {
        public string ClassrunTitle { get; set; }

        public string CourseTitle { get; set; }
    }
}
