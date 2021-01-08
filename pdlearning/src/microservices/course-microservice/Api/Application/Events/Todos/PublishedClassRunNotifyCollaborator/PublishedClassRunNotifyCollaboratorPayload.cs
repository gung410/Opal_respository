namespace Microservice.Course.Application.Events.Todos
{
    public class PublishedClassRunNotifyCollaboratorPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string CoursePDArea { get; set; }
    }
}
