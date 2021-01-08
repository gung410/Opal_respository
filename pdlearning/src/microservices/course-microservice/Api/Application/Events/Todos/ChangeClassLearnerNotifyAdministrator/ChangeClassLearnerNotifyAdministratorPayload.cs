namespace Microservice.Course.Application.Events.Todos
{
    public class ChangeClassLearnerNotifyAdministratorPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string LearnerName { get; set; }
    }
}
