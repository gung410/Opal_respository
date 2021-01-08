namespace Microservice.Course.Application.Events.Todos
{
    public class WithdrawClassrunNotifyAdministratorPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string CourseRunTitle { get; set; }
    }
}
