using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.LnaForm.Application.Events.EventPayloads;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.LnaForm.Application.Events.EventHandlers
{
    public class FormParticipantNotifyEventHandler : BaseFormTodoRegistrationEventHandler<FormParticipantNotifyEvent, NotifyFormParticipantPayload>
    {
        public FormParticipantNotifyEventHandler(
          IOptions<RabbitMQOptions> options,
          IOpalMessageProducer producer,
          IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override List<ReceiverDto> GetAssignedTo(FormParticipantNotifyEvent @event)
        {
            return new List<ReceiverDto>()
            {
                 new ReceiverDto() { UserId = @event.ParticipantId.ToString() }
            };
        }

        protected override NotifyFormParticipantPayload GetPayload(FormParticipantNotifyEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReminderByDto> GetReminderBy(FormParticipantNotifyEvent @event)
        {
            return new List<ReminderByDto>()
            {
                 @event.ReminderByConditions
            };
        }
    }
}
