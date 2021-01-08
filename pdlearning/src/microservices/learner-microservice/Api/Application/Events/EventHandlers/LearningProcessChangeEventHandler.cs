using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Learner.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Learner.Application.Events.EventHandlers
{
    public class LearningProcessChangeEventHandler : OutboxOpalMqEventHandler<LearningProcessChangeEvent, LearningProcessModel>
    {
        public LearningProcessChangeEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue outboxQueue) : base(options, userContext, outboxQueue)
        {
        }

        protected override LearningProcessModel GetMQMessagePayload(LearningProcessChangeEvent @event)
        {
            return @event.LearningProcess;
        }
    }
}
