namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedRegistrationNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string CourseCode { get; set; }

        public string Comment { get; set; }
    }
}
