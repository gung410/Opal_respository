using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Events
{
    public abstract class BaseContentTodoRegistrationEvent : BaseThunderEvent
    {
        public string TaskURI { get; set; }

        public string Subject { get; set; }

        public string Template { get; set; }
    }
}
