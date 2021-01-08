using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseNotifyOwnerEventHandler : BaseTodoEventHandler<ApprovedCourseNotifyOwnerEvent, ApprovedCourseNotifyOwnerPayload>
    {
        public ApprovedCourseNotifyOwnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override ApprovedCourseNotifyOwnerPayload GetPayload(ApprovedCourseNotifyOwnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(ApprovedCourseNotifyOwnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
