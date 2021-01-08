namespace Microservice.Content.Application.Events
{
    public class NotifyContentExpiredPayload
    {
        public string RecipientName { get; set; }

        public string ContentName { get; set; }

        public string ContentDetailURL { get; set; }

        // this fields use for in app notification setting, please don't remove it
        public string ActionName { get; set; }

        public string ActionUrl { get; set; }
    }
}
