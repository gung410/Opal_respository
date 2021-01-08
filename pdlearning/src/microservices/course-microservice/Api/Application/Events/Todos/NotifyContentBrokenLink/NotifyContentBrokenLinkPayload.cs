namespace Microservice.Course.Application.Events.Todos
{
    public class NotifyContentBrokenLinkPayload : BaseTodoEventPayload
    {
        public string AssetOwnerName { get; set; }

        public string AssetName { get; set; }
    }
}
