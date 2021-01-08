using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedCourseNotifyOwnerEventHandler : BaseTodoEventHandler<RejectedCourseNotifyOwnerEvent, RejectedCourseNotifyOwnerPayload>
    {
        public RejectedCourseNotifyOwnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override RejectedCourseNotifyOwnerPayload GetPayload(RejectedCourseNotifyOwnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(RejectedCourseNotifyOwnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
