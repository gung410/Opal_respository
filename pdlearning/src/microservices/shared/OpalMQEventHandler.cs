using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Conexus.Opal.Connector.RabbitMQ.Extensions
{
    /// <summary>
    /// Base class for sending message (event) to OPAL Rabbit MQ. This logic is specialized for OPAL2.0 project.
    /// Because the message can be in differrent shape, for that reason, I don't want this class to be in Conexus.Opal.Connector.RabbitMQ.
    /// This leads to a litle bit difficult to include/use this class -> That's the purpose.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="TMQPayload">The MQ payload.</typeparam>
    public abstract class OpalMQEventHandler<TEvent, TMQPayload> : BaseThunderEventHandler<TEvent> where TEvent : BaseThunderEvent where TMQPayload : class
    {
        private readonly RabbitMQOptions _options;
        private readonly IOpalMessageProducer _producer;
        private readonly IUserContext _userContext;

        protected OpalMQEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer, IUserContext userContext)
        {
            _options = options.Value;
            _producer = producer;
            _userContext = userContext;
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

            await _producer.SendAsync(message, _options.DefaultEventExchange, @event.GetRoutingKey());
        }
    }
}
