namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawalRejectedNotifyLearnerByApproverPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassrunTitle { get; set; }
    }
}
