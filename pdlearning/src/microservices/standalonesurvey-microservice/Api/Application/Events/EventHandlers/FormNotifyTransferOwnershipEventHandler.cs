using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Application.Events.EventPayloads;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.StandaloneSurvey.Application.Events.EventHandlers
{
    public class FormNotifyTransferOwnershipEventHandler : BaseFormTodoRegistrationEventHandler<FormNotifyTransferOwnershipEvent, NotifyTransferOwnershipPayload>
    {
        public FormNotifyTransferOwnershipEventHandler(
          IOptions<RabbitMQOptions> options,
          IOpalMessageProducer producer,
          IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override List<ReceiverDto> GetAssignedTo(FormNotifyTransferOwnershipEvent @event)
        {
            return new List<ReceiverDto>()
            {
                 new ReceiverDto() { UserId = @event.NewOwnerId.ToString() }
            };
        }

        protected override NotifyTransferOwnershipPayload GetPayload(FormNotifyTransferOwnershipEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReminderByDto> GetReminderBy(FormNotifyTransferOwnershipEvent @event)
        {
            return new List<ReminderByDto>()
            {
                 @event.ReminderByConditions
            };
        }
    }
}
