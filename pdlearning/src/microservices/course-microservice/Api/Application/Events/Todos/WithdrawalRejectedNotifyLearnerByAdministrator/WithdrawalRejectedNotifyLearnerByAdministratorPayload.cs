namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawalRejectedNotifyLearnerByAdministratorPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassrunTitle { get; set; }
    }
}
