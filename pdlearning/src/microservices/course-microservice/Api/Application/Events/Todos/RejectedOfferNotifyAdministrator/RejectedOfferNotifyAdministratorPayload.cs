namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedOfferNotifyAdministratorPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }
    }
}
