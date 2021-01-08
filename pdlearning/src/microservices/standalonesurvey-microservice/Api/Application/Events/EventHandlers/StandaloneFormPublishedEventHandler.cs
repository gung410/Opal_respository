using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.StandaloneSurvey.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.StandaloneSurvey.Application.Events.EventHandlers
{
    public class StandaloneFormPublishedEventHandler : OutboxOpalMqEventHandler<StandaloneFormPublishedEvent, StandaloneFormPublishedEventModel>
    {
        public StandaloneFormPublishedEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue queue) : base(options, userContext, queue)
        {
        }

        protected override StandaloneFormPublishedEventModel GetMQMessagePayload(StandaloneFormPublishedEvent @event)
        {
            return @event.Model;
        }
    }
}
