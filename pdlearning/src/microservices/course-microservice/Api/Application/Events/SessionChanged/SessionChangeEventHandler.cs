using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.AssociatedEntities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class SessionChangeEventHandler : OutboxOpalMqEventHandler<SessionChangeEvent, SessionAssociatedEntity>
    {
        public SessionChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override SessionAssociatedEntity GetMQMessagePayload(SessionChangeEvent @event)
        {
            return @event.Session;
        }
    }
}
