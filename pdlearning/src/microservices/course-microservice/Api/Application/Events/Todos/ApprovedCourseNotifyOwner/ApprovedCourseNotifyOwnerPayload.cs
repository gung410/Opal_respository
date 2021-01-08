namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseNotifyOwnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string Comment { get; set; }
    }
}
