using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.OutboxPattern;
using Microservice.Application.Events.EventPayloads;
using Microservice.Content.Application.Events;
using Microsoft.Extensions.Options;

namespace Microservice.Form.Application.Events.EventHandlers
{
    public class NotifyContentTransferOwnershipEventHandler : BaseContentTodoRegistrationEventHandler<NotifyContentTransferOwnershipEvent, NotifyTransferOwnershipPayload>
    {
        public NotifyContentTransferOwnershipEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer) : base(options, producer)
        {
        }

        protected override List<ReceiverDto> GetAssignedTo(NotifyContentTransferOwnershipEvent @event)
        {
            return new List<ReceiverDto>()
            {
                 new ReceiverDto() { UserId = @event.NewOwnerId.ToString() }
            };
        }

        protected override DateTime? GetDeadlineUTC(NotifyContentTransferOwnershipEvent @event)
        {
            return null;
        }

        protected override NotifyTransferOwnershipPayload GetPayload(NotifyContentTransferOwnershipEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReminderByDto> GetReminderBy(NotifyContentTransferOwnershipEvent @event)
        {
            return new List<ReminderByDto>()
            {
                 @event.ReminderByConditions
            };
        }
    }
}
