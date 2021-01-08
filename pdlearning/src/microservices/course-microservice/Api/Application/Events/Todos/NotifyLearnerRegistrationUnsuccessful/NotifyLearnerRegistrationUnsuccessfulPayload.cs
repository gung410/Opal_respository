namespace Microservice.Course.Application.Events.Todos
{
    public class NotifyLearnerRegistrationUnsuccessfulPayload : BaseTodoEventPayload
    {
        public string LearnerName { get; set; }

        public string CourseName { get; set; }

        public string ClassName { get; set; }

        public string ClassStartDate { get; set; }
    }
}
