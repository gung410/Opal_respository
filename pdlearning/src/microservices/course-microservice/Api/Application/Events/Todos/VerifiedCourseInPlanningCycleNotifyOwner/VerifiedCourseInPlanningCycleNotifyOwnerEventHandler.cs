using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class VerifiedCourseInPlanningCycleNotifyOwnerEventHandler : BaseTodoEventHandler<VerifiedCourseInPlanningCycleNotifyOwnerEvent, VerifiedCourseInPlanningCycleNotifyOwnerPayload>
    {
        public VerifiedCourseInPlanningCycleNotifyOwnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override VerifiedCourseInPlanningCycleNotifyOwnerPayload GetPayload(VerifiedCourseInPlanningCycleNotifyOwnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(VerifiedCourseInPlanningCycleNotifyOwnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
