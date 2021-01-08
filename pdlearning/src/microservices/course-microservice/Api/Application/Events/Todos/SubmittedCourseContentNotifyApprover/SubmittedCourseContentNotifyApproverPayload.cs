namespace Microservice.Course.Application.Events.Todos
{
    public class SubmittedCourseContentNotifyApproverPayload : BaseTodoEventPayload
    {
        public string CourseName { get; set; }
    }
}
