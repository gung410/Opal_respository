namespace Microservice.Content.Application.Events
{
    public class PreNotifyContentExpiredPayload
    {
        public string Subject { get; set; }

        public string Template { get; set; }

        public string RecipientName { get; set; }

        public string ContentName { get; set; }

        public string ContentDetailURL { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        // this fields use for in app notification setting, please don't remove it
        public string ActionName { get; set; }

        public string ActionUrl { get; set; }
    }
}
