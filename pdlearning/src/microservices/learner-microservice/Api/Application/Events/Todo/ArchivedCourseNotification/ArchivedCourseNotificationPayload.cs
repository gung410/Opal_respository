namespace Microservice.Learner.Application.Events.Todo
{
    public class ArchivedCourseNotificationPayload : BaseTodoEventPayload
    {
        public string CourseType { get; set; }

        public string CourseTitle { get; set; }
    }
}
