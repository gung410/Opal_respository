using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Conexus.Opal.Microservice.Tagging.Events.ResourceSavedEvent
{
    public class SaveResourceEventHandler : OpalMQEventHandler<SaveResourceEvent, Resource>
    {
        public SaveResourceEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer, IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override Resource GetMQMessagePayload(SaveResourceEvent @event)
        {
            return @event.Resource;
        }
    }
}
