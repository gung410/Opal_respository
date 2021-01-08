using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Events
{
    public abstract class BaseTodoRegistrationEvent : BaseThunderEvent
    {
        public string TaskURI { get; set; }

        public string Subject { get; set; }

        public string Template { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
