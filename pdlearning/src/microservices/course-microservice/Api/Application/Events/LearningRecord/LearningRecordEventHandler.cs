using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class LearningRecordEventHandler : BaseThunderEventHandler<LearningRecordEvent>
    {
        public const string RoutingKey = "learningrecord.post";

        private readonly IOutboxQueue _outboxQueue;
        private readonly RabbitMQOptions _options;

        public LearningRecordEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue outboxQueue)
        {
            _options = options.Value;
            _outboxQueue = outboxQueue;
        }

        protected override async Task HandleAsync(LearningRecordEvent @event, CancellationToken cancellationToken)
        {
            await _outboxQueue.QueueMessageAsync(new QueueMessage(RoutingKey, @event, _options.DefaultIntegrationExchange));
        }
    }
}
