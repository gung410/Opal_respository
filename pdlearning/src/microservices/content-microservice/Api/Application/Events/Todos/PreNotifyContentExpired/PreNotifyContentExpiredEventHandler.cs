using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.Options;

namespace Microservice.Content.Application.Events
{
    public class PreNotifyContentExpiredEventHandler : BaseContentTodoRegistrationEventHandler<PreNotifyContentExpiredEvent, PreNotifyContentExpiredPayload>
    {
        public PreNotifyContentExpiredEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer) : base(options, producer)
        {
        }

        protected override PreNotifyContentExpiredPayload GetPayload(PreNotifyContentExpiredEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetAssignedTo(PreNotifyContentExpiredEvent @event)
        {
            return new List<ReceiverDto>
            {
                 new ReceiverDto { UserId = @event.ContentCreatorId.ToString() }
            };
        }

        protected override List<ReminderByDto> GetReminderBy(PreNotifyContentExpiredEvent @event)
        {
            return new List<ReminderByDto>
            {
                 @event.ReminderByConditions
            };
        }

        protected override DateTime? GetDeadlineUTC(PreNotifyContentExpiredEvent @event)
        {
            return null;
        }
    }
}
