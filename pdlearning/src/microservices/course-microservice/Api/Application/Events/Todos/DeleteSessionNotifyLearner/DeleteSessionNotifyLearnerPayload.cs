namespace Microservice.Course.Application.Events.Todos
{
    public class DeleteSessionNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassrunTitle { get; set; }

        public string SessionDate { get; set; }

        public string SessionStartTime { get; set; }

        public string SessionEndTime { get; set; }

        public string SessionVenue { get; set; }
    }
}
