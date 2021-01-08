using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Events
{
    public class BaseContentCommunicationEvent : BaseThunderEvent
    {
        public string Subject { get; set; }

        public string DisplayMessage { get; set; }

        public string UserId { get; set; }
    }
}
