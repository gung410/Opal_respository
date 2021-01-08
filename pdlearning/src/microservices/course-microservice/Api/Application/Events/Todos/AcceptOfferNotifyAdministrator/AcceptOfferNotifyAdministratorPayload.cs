namespace Microservice.Course.Application.Events.Todos
{
    public class AcceptOfferNotifyAdministratorPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }
    }
}
