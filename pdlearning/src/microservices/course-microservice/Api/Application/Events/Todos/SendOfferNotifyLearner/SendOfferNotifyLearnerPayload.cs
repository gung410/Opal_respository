namespace Microservice.Course.Application.Events.Todos
{
    public class SendOfferNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }
    }
}
