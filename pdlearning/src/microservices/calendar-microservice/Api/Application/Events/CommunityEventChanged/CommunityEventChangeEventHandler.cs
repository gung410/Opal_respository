using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Calendar.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Calendar.Application.Events
{
    public class CommunityEventChangeEventHandler : OutboxOpalMqEventHandler<CommunityEventChangeEvent, CommunityEvent>
    {
        public CommunityEventChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override CommunityEvent GetMQMessagePayload(CommunityEventChangeEvent @event)
        {
            return @event.Event;
        }
    }
}
