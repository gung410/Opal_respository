namespace Microservice.Course.Application.Events.Todos
{
    public class SendCourseNominationAnnouncementNotifyPayload : BaseTodoEventPayload
    {
        public string UserName { get; set; }

        public string CourseTitle { get; set; }
    }
}
