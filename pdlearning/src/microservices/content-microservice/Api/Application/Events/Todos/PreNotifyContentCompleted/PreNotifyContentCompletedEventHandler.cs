using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Content.Application.Events
{
    public class PreNotifyContentCompletedEventHandler : BaseTodoCompleteEventHandler<PreNotifyContentCompletedEvent>
    {
        public PreNotifyContentCompletedEventHandler(
            IOutboxQueue outboxQueue,
            IOptions<RabbitMQOptions> rabbitMQOptions) : base(outboxQueue, rabbitMQOptions)
        {
        }
    }
}
