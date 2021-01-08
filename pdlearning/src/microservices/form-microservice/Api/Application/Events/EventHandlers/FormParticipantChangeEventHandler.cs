using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Form.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Application.Events.EventHandlers
{
    public class FormParticipantChangeEventHandler : OutboxOpalMqEventHandler<FormParticipantChangeEvent, FormParticipant>
    {
        public FormParticipantChangeEventHandler(
            IOptions<RabbitMQOptions> options,
            IUserContext userContext,
            IOutboxQueue queue)
            : base(options, userContext, queue)
        {
        }

        protected override FormParticipant GetMQMessagePayload(FormParticipantChangeEvent @event)
        {
            return @event.FormParticipant;
        }
    }
}
