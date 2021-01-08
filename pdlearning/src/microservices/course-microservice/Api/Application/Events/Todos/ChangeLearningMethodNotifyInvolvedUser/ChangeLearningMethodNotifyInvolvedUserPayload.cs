namespace Microservice.Course.Application.Events.Todos
{
    public class ChangeLearningMethodNotifyInvolvedUserPayload : BaseTodoEventPayload
    {
        public string CourseName { get; set; }

        public string ClassRunName { get; set; }

        public string SessionTitle { get; set; }

        public string SessionVenue { get; set; }

        public string MessageIfIsLearningOnline { get; set; }

        public string ChangeLearningMethodText { get; set; }
    }
}
