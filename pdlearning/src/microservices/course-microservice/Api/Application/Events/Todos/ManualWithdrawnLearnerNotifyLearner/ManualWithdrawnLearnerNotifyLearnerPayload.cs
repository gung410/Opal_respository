namespace Microservice.Course.Application.Events.Todos
{
    public class ManualWithdrawnLearnerNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CAName { get; set; }

        public string CAEmail { get; set; }

        public string CourseTitle { get; set; }
    }
}
