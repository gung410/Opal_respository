using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Conexus.Opal.Connector.RabbitMQ.Extensions
{
    /// <summary>
    /// The base handler for sending rabbitMq message (event) to OPAL Rabbit MQ with supporting the Outbox pattern.
    /// This logic is specialized for the OPAL2.0 project.
    /// Because the message can be in different shapes, for that reason, this class didn't include in Conexus.Opal.Connector.RabbitMQ.
    /// This leads to a little bit difficult to include/use this class -> That's the purpose.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="TMQPayload">The MQ payload.</typeparam>
    public abstract class OutboxOpalMqEventHandler<TEvent, TMQPayload> : BaseThunderEventHandler<TEvent> where TEvent : BaseThunderEvent where TMQPayload : class
    {
        private readonly RabbitMQOptions _options;
        private readonly IUserContext _userContext;
        private readonly IOutboxQueue _outboxQueue;

        protected OutboxOpalMqEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue outboxQueue)
        {
            _options = options.Value;
            _userContext = userContext;
            _outboxQueue = outboxQueue;
        }

        protected virtual string MQMessageType { get; } = OpalMQMessageType.Event;

        protected abstract TMQPayload GetMQMessagePayload(TEvent @event);

        protected override async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
        {
            var message = new OpalMQMessage<TMQPayload>
            {
                Type = MQMessageType,
                Name = @event.GetRoutingKey(),
                Routing = new OpalMQMessageRouting
                {
                    Action = @event.GetRoutingKey(),
                },
                Payload = new OpalMQMessagePayload<TMQPayload>
                {
                    Identity = new OpalMQMessageIdentity
                    {
                        SourceIp = _userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                        UserId = _userContext.GetValue<string>(CommonUserContextKeys.UserId)
                    },
                    Body = GetMQMessagePayload(@event)
                }
            };

            var queueMessage = new QueueMessage(@event.GetRoutingKey(), message, _options.DefaultEventExchange);
            await _outboxQueue.QueueMessageAsync(queueMessage);
        }
    }
}
