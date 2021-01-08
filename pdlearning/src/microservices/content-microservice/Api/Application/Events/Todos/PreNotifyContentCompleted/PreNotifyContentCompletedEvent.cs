using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.Content.Application.Events
{
    public class PreNotifyContentCompletedEvent : BaseTodoCompleteEvent
    {
        public PreNotifyContentCompletedEvent(
            Guid digitalContentId,
            Status status)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:pre-notified-content-expried:{digitalContentId}";
            Status = status;
        }
    }
}
