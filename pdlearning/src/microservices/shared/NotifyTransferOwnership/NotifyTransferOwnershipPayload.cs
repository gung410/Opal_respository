namespace Microservice.Application.Events.EventPayloads
{
    public class NotifyTransferOwnershipPayload
    {
        public string NewOwnerName { get; set; }

        public string OwnerName { get; set; }

        public string ObjectName { get; set; }

        public string ObjectDetailUrl { get; set; }

        // this fields use for in app notification setting, please don't remove it
        public string ActionName { get; set; }

        public string ActionUrl { get; set; }
    }
}
