using System;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Events.Todos
{
    public abstract class BaseTodoEvent<TPayLoad> : BaseThunderEvent where TPayLoad : BaseTodoEventPayload
    {
        public string TaskURI { get; set; }

        public string Subject { get; set; }

        public string Template { get; set; }

        public string Message { get; set; }

        public string PlainText { get; set; }

        public Guid CreatedBy { get; set; }

        public TPayLoad Payload { get; set; }
    }

    public abstract class BaseTodoEventPayload
    {
        public string ActionName { get; set; } = string.Empty;

        public string ActionUrl { get; set; } = string.Empty;

        public string ObjectType { get; set; } = string.Empty;

        public Guid ObjectId { get; set; } = Guid.Empty;
    }
}

#pragma warning restore SA1402 // File may only contain a single type
