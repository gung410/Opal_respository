using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microservice.BrokenLink.Application.Events;
using Microsoft.Extensions.Options;

namespace Microservice.Content.Application.Events
{
    public class NotifyBrokenLinkEventHandler : BaseTodoRegistrationEventHandler<NotifyBrokenLinkEvent, NotifyBrokenLinkPayload>
    {
        public NotifyBrokenLinkEventHandler(IOutboxQueue outboxQueue, IOptions<RabbitMQOptions> rabbitMQOptions) : base(outboxQueue, rabbitMQOptions)
        {
        }

        protected override NotifyBrokenLinkPayload GetPayload(NotifyBrokenLinkEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetAssignedTo(NotifyBrokenLinkEvent @event)
        {
            return new List<ReceiverDto>
            {
                 new ReceiverDto { UserId = @event.OwnerId.ToString() }
            };
        }

        protected override List<ReminderByDto> GetReminderBy(NotifyBrokenLinkEvent @event)
        {
            return new List<ReminderByDto>
            {
                 @event.ReminderByConditions
            };
        }

        protected override DateTime? GetDeadlineUTC(NotifyBrokenLinkEvent @event)
        {
            return null;
        }
    }
}
