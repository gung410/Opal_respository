using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Content.Application.Events
{
    public class MigrateSearchContentDataEventHandler : OutboxOpalMqEventHandler<MigrateSearchContentDataEvent, DigitalContentModel>
    {
        public MigrateSearchContentDataEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue queue) : base(options, userContext, queue)
        {
        }

        protected override DigitalContentModel GetMQMessagePayload(MigrateSearchContentDataEvent @event)
        {
            return @event.Model;
        }
    }
}
