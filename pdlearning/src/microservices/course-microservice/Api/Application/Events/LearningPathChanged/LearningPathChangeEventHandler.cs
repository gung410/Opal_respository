using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class LearningPathChangeEventHandler : OutboxOpalMqEventHandler<LearningPathChangeEvent, LearningPathModel>
    {
        public LearningPathChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override LearningPathModel GetMQMessagePayload(LearningPathChangeEvent @event)
        {
            return @event.LearningPath;
        }
    }
}
