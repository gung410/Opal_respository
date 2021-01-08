using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Conexus.Opal.Connector.RabbitMQ.Extensions
{
    /// <summary>
    /// The base handler for sending rabbitMq message (event) to OPAL Rabbit MQ with supporting the Outbox pattern.
    /// Difference from <see cref="OutboxOpalMqEventHandler"/>, this base handler open a new transaction for each message automatically.
    /// This logic is specialized for the OPAL2.0 project.
    /// Because the message can be in different shapes, for that reason, this class didn't include in Conexus.Opal.Connector.RabbitMQ.
    /// This leads to a little bit difficult to include/use this class -> That's the purpose.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="TMQPayload">The MQ payload.</typeparam>
    public abstract class ScopedOutboxOpalMqEventHandler<TEvent, TMQPayload> : BaseThunderEventHandler<TEvent> where TEvent : BaseThunderEvent where TMQPayload : class
    {
        private readonly RabbitMQOptions _options;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserContext _userContext;
        private readonly IOutboxQueue _outboxQueue;

        protected ScopedOutboxOpalMqEventHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IOptions<RabbitMQOptions> options,
            IUserContext userContext,
            IOutboxQueue outboxQueue)
        {
            _options = options.Value;
            _unitOfWorkManager = unitOfWorkManager;
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

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await _outboxQueue.QueueMessageAsync(queueMessage);

                await uow.CompleteAsync();
            }
        }
    }
}
