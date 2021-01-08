using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Events
{
    public abstract class BaseRegistrationTodoEvent : BaseThunderEvent
    {
        public string TaskURI { get; set; }

        public string Subject { get; set; }

        public string Template { get; set; }
    }
}
