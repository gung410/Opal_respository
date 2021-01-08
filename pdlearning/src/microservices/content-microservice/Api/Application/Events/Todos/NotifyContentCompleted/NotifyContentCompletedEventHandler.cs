using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Content.Application.Events
{
    public class NotifyContentCompletedEventHandler : BaseTodoCompleteEventHandler<NotifyContentCompletedEvent>
    {
        public NotifyContentCompletedEventHandler(
            IOutboxQueue outboxQueue,
            IOptions<RabbitMQOptions> rabbitMQOptions)
            : base(outboxQueue, rabbitMQOptions)
        {
        }
    }
}
