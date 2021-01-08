using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application
{
    public abstract class BaseTodoCompleteEventHandler<T> : BaseThunderEventHandler<T> where T : BaseTodoCompleteEvent
    {
        private readonly IOutboxQueue _outboxQueue;
        private readonly RabbitMQOptions _rabbitMQOptions;

        protected BaseTodoCompleteEventHandler(
            IOutboxQueue outboxQueue,
            IOptions<RabbitMQOptions> rabbitMQOptions)
        {
            _outboxQueue = outboxQueue;
            _rabbitMQOptions = rabbitMQOptions.Value;
        }

        protected override async Task HandleAsync(T @event, CancellationToken cancellationToken)
        {
            var message = new TodoCompleteMQMessage
            {
                TaskURI = @event.TaskURI,
                Status = @event.Status
            };

            await _outboxQueue.QueueMessageAsync(new QueueMessage(TodoCompleteMQMessage.RoutingKey, message, _rabbitMQOptions.DefaultIntegrationExchange));
        }
    }
}
