namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCancellationClassRunNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string ClassTitle { get; set; }

        public string CourseName { get; set; }

        public string CourseCode { get; set; }

        public string CourseTitle { get; set; }
    }
}
