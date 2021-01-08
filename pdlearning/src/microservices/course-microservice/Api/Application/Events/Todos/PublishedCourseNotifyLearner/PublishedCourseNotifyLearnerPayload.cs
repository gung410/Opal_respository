namespace Microservice.Course.Application.Events.Todos
{
    public class PublishedCourseNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string CoursePDArea { get; set; }
    }
}
