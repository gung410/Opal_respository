namespace Microservice.Badge.Application.TodoEvents
{
    public class AchievedBadgesNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string ParamBadgeText { get; set; }
    }
}
