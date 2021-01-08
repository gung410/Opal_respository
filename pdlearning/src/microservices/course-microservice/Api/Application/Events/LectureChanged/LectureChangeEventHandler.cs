using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class LectureChangeEventHandler : OutboxOpalMqEventHandler<LectureChangeEvent, LectureModel>
    {
        public LectureChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override LectureModel GetMQMessagePayload(LectureChangeEvent @event)
        {
            return @event.LectureModel;
        }
    }
}
