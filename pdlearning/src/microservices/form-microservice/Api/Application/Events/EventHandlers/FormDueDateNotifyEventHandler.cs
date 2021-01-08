using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Form.Application.Events.EventPayloads;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Application.Events.EventHandlers
{
    public class FormDueDateNotifyEventHandler : BaseFormTodoRegistrationEventHandler<FormDueDateNotifyEvent, NotifyFormDueDatePayload>
    {
        public FormDueDateNotifyEventHandler(
          IOptions<RabbitMQOptions> options,
          IOpalMessageProducer producer,
          IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override List<ReceiverDto> GetAssignedTo(FormDueDateNotifyEvent @event)
        {
            return new List<ReceiverDto>()
            {
                 new ReceiverDto() { UserId = @event.ReceivertId.ToString() }
            };
        }

        protected override NotifyFormDueDatePayload GetPayload(FormDueDateNotifyEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReminderByDto> GetReminderBy(FormDueDateNotifyEvent @event)
        {
            return new List<ReminderByDto>()
            {
                 @event.ReminderByConditions
            };
        }
    }
}
