using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class ResourcesNotReferencedEventHandler : OutboxOpalMqEventHandler<ResourcesNotReferencedEvent, ResourcesNotReferencedEventBody>
    {
        public ResourcesNotReferencedEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override ResourcesNotReferencedEventBody GetMQMessagePayload(ResourcesNotReferencedEvent @event)
        {
            return @event.Body;
        }
    }
}
