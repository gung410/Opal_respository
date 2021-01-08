using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class ECertificateEventHandler : BaseThunderEventHandler<ECertificateEvent>
    {
        public const string RoutingKey = "learningrecord.post.ecertificate";

        private readonly RabbitMQOptions _options;
        private readonly IOutboxQueue _outboxQueue;

        public ECertificateEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue outboxQueue)
        {
            _options = options.Value;
            _outboxQueue = outboxQueue;
        }

        protected override async Task HandleAsync(ECertificateEvent @event, CancellationToken cancellationToken)
        {
            await _outboxQueue.QueueMessageAsync(new QueueMessage(RoutingKey, @event, _options.DefaultIntegrationExchange));
        }
    }
}
