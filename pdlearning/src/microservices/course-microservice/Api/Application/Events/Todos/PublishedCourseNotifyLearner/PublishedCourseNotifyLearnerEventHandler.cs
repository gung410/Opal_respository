using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class PublishedCourseForLearnerEventHandler : BaseTodoEventHandler<PublishedCourseNotifyLearnerEvent, PublishedCourseNotifyLearnerPayload>
    {
        public PublishedCourseForLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override PublishedCourseNotifyLearnerPayload GetPayload(PublishedCourseNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(PublishedCourseNotifyLearnerEvent @event)
        {
            return null;
        }
    }
}
