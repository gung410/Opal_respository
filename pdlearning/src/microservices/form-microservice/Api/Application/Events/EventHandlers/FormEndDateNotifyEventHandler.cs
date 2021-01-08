using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Form.Application.Events.EventPayloads;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Application.Events.EventHandlers
{
    public class FormEndDateNotifyEventHandler : BaseFormTodoRegistrationEventHandler<FormEndDateNotifyEvent, NotifyFormDueDatePayload>
    {
        public FormEndDateNotifyEventHandler(
          IOptions<RabbitMQOptions> options,
          IOpalMessageProducer producer,
          IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override List<ReceiverDto> GetAssignedTo(FormEndDateNotifyEvent @event)
        {
            return new List<ReceiverDto>()
            {
                 new ReceiverDto() { UserId = @event.ReceivertId.ToString() }
            };
        }

        protected override NotifyFormDueDatePayload GetPayload(FormEndDateNotifyEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReminderByDto> GetReminderBy(FormEndDateNotifyEvent @event)
        {
            return new List<ReminderByDto>()
            {
                 @event.ReminderByConditions
            };
        }
    }
}
