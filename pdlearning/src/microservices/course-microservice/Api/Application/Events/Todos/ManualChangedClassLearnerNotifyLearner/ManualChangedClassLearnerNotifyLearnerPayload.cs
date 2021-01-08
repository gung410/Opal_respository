namespace Microservice.Course.Application.Events.Todos
{
    public class ManualChangedClassLearnerNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CAName { get; set; }

        public string CAEmail { get; set; }

        public string CourseTitle { get; set; }
    }
}
