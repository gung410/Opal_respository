namespace Microservice.Course.Application.Events.Todos
{
    public class SendAnnouncementNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string UserName { get; set; }

        public string CourseTitle { get; set; }

        public string ClassTitle { get; set; }

        public string AnnouncementTitle { get; set; }

        public string AnnouncementContent { get; set; }
    }
}
