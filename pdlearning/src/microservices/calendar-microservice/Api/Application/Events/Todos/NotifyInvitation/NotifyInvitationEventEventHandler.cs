using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Calendar.Application.Events
{
    public class NotifyInvitationEventEventHandler : BaseTodoRegistrationEventHandler<NotifyInvitationEvent, NotifyInvitationPayload>
    {
        public NotifyInvitationEventEventHandler(IOutboxQueue outboxQueue, IOptions<RabbitMQOptions> rabbitMQOptions) : base(outboxQueue, rabbitMQOptions)
        {
        }

        protected override NotifyInvitationPayload GetPayload(NotifyInvitationEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetAssignedTo(NotifyInvitationEvent @event)
        {
            return new List<ReceiverDto>
            {
                 new ReceiverDto { UserId = @event.AttendeeId.ToString() }
            };
        }

        protected override List<ReminderByDto> GetReminderBy(NotifyInvitationEvent @event)
        {
            return new List<ReminderByDto>
            {
                 @event.ReminderByConditions
            };
        }

        protected override DateTime? GetDeadlineUTC(NotifyInvitationEvent @event)
        {
            return null;
        }

        protected override string GetCreatedBy(NotifyInvitationEvent @event)
        {
            return @event.CreatedBy.ToString();
        }
    }
}
