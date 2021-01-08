namespace Microservice.Course.Application.Events.Todos
{
    public class SubmittedWithdrawClassrunNotifyApproverPayload : BaseTodoEventPayload
    {
        public string LearnerName { get; set; }

        public string CourseTitle { get; set; }
    }
}
