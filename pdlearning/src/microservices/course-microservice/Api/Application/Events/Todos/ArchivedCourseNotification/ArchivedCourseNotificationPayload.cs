namespace Microservice.Course.Application.Events.Todos
{
    public class ArchivedCourseNotificationPayload : BaseTodoEventPayload
    {
        public string CourseType { get; set; }

        public string CourseTitle { get; set; }
    }
}