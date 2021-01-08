using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Events
{
    public abstract class BaseContentCommunicationEventHandler<T> : BaseThunderEventHandler<T> where T : BaseContentCommunicationEvent
    {
        private readonly IUserContext _userContext;
        private readonly IOutboxQueue _outboxQueue;

        protected BaseContentCommunicationEventHandler(IUserContext userContext, IOutboxQueue outboxQueue)
        {
            _userContext = userContext;
            _outboxQueue = outboxQueue;
        }

        protected override async Task HandleAsync(T @event, CancellationToken cancellationToken)
        {
            var routingKey = "reminder.communication.send.message.success";
            var communicationMQMessage = new CommunicationMQMessage
            {
                Created = DateTime.UtcNow,
                Routing = new OpalMQMessageRouting
                {
                    Action = routingKey
                },
                Type = "command",
                Version = "1.0",
                Payload = new OpalMQMessagePayload<Body>
                {
                    Identity = new OpalMQMessageIdentity
                    {
                        UserId = _userContext.GetValue<string>(CommonUserContextKeys.UserId)
                    },
                    Body = new Body
                    {
                        Message = new Message
                        {
                            Subject = @event.Subject,
                            DisplayMessage = @event.DisplayMessage
                        },
                        Recipient = new Recipient
                        {
                            UserIds = new HashSet<string> { @event.UserId },
                            cxToken = "3001:2052",
                        }
                    }
                }
            };

            await _outboxQueue.QueueMessageAsync(new QueueMessage(routingKey, communicationMQMessage));
        }
    }
}
