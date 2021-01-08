namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedRescheduleClassRunNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string ClassTitle { get; set; }

        public string CourseName { get; set; }

        public string CourseCode { get; set; }

        public string RevisedStartDate { get; set; }

        public string RevisedEndDate { get; set; }
    }
}
