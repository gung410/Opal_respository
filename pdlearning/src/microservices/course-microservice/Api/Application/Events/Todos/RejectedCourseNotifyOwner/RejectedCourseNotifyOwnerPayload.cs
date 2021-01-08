namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedCourseNotifyOwnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string Comment { get; set; }
    }
}
