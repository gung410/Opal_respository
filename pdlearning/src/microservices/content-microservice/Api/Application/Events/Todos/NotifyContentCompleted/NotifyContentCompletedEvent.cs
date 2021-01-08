using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.Content.Application.Events
{
    public class NotifyContentCompletedEvent : BaseTodoCompleteEvent
    {
        public NotifyContentCompletedEvent(
            Guid digitalContentId,
            Status status)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-content-expried:{digitalContentId}";
            Status = status;
        }
    }
}
