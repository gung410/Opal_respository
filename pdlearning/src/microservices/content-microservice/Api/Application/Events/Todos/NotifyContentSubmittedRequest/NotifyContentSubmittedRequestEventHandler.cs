using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.Options;

namespace Microservice.Content.Application.Events
{
    public class NotifyContentSubmittedRequestEventHandler : BaseContentTodoRegistrationEventHandler<NotifyContentSubmittedRequestEvent, NotifyContentSubmittedRequestPayload>
    {
        public NotifyContentSubmittedRequestEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer) : base(options, producer)
        {
        }

        protected override List<ReceiverDto> GetAssignedTo(NotifyContentSubmittedRequestEvent @event)
        {
            return new List<ReceiverDto>()
            {
                 new ReceiverDto() { UserId = @event.ReceiverId.ToString() }
            };
        }

        protected override DateTime? GetDeadlineUTC(NotifyContentSubmittedRequestEvent @event)
        {
            return null;
        }

        protected override NotifyContentSubmittedRequestPayload GetPayload(NotifyContentSubmittedRequestEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReminderByDto> GetReminderBy(NotifyContentSubmittedRequestEvent @event)
        {
            return new List<ReminderByDto>()
            {
                 @event.ReminderByConditions
            };
        }
    }
}
