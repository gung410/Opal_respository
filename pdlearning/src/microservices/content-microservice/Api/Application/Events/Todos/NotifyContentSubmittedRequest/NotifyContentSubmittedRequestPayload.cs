namespace Microservice.Content.Application.Events
{
    public class NotifyContentSubmittedRequestPayload
    {
        public string RecipientName { get; set; }

        public string ContentName { get; set; }

        public string CreatorName { get; set; }

        public string AOName { get; set; }

        public string Comment { get; set; }

        public string ContentDetailUrl { get; set; }
    }
}
