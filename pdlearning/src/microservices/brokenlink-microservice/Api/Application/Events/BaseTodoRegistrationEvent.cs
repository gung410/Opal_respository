using Thunder.Platform.Cqrs;

namespace Microservice.BrokenLink.Application.Events
{
    public abstract class BaseTodoRegistrationEvent : BaseThunderEvent
    {
        public string TaskURI { get; set; }

        public string Subject { get; set; }

        public string Template { get; set; }
    }
}
