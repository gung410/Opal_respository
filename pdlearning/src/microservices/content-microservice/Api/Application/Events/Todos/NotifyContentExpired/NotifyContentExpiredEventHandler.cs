using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Content.Application.Events
{
    public class NotifyContentExpiredEventHandler : BaseContentTodoRegistrationEventHandler<NotifyContentExpiredEvent, NotifyContentExpiredPayload>
    {
        public NotifyContentExpiredEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer) : base(options, producer)
        {
        }

        protected override NotifyContentExpiredPayload GetPayload(NotifyContentExpiredEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetAssignedTo(NotifyContentExpiredEvent @event)
        {
            return new List<ReceiverDto>
            {
                 new ReceiverDto { UserId = @event.ContentCreatorId.ToString() }
            };
        }

        protected override List<ReminderByDto> GetReminderBy(NotifyContentExpiredEvent @event)
        {
            return new List<ReminderByDto>
            {
                 @event.ReminderByConditions
            };
        }

        protected override DateTime? GetDeadlineUTC(NotifyContentExpiredEvent @event)
        {
            return null;
        }
    }
}
