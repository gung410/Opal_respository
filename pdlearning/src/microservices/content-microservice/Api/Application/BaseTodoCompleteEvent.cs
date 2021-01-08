using Conexus.Opal.Connector.RabbitMQ.Contract;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application
{
    public abstract class BaseTodoCompleteEvent : BaseThunderEvent
    {
        public string TaskURI { get; set; }

        public Status Status { get; set; }
    }
}
