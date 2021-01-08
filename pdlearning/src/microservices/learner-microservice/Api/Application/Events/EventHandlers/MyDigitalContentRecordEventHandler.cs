using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events.EventHandlers
{
    public class MyDigitalContentRecordEventHandler : BaseThunderEventHandler<MyDigitalContentRecordEvent>
    {
        private readonly IOutboxQueue _outboxQueue;
        private readonly RabbitMQOptions _options;

        public MyDigitalContentRecordEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue outboxQueue)
        {
            _options = options.Value;
            _outboxQueue = outboxQueue;
        }

        protected override Task HandleAsync(MyDigitalContentRecordEvent @event, CancellationToken cancellationToken)
        {
            return _outboxQueue.QueueMessageAsync(new QueueMessage(@event.GetRoutingKey(), @event, _options.DefaultIntegrationExchange));
        }
    }
}
