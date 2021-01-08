using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class RegistrationChangeEventHandler : OutboxOpalMqEventHandler<RegistrationChangeEvent, RegistrationAssociatedEntity>
    {
        public RegistrationChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override RegistrationAssociatedEntity GetMQMessagePayload(RegistrationChangeEvent @event)
        {
            return @event.RegistrationAssociatedEntity;
        }
    }
}
