using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Form.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Application.Events.EventHandlers
{
    public class MigrateSearchFormDataEventHandler : OutboxOpalMqEventHandler<MigrateSearchFormDataEvent, FormModel>
    {
        public MigrateSearchFormDataEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue queue) : base(options, userContext, queue)
        {
        }

        protected override FormModel GetMQMessagePayload(MigrateSearchFormDataEvent @event)
        {
            return @event.Model;
        }
    }
}
