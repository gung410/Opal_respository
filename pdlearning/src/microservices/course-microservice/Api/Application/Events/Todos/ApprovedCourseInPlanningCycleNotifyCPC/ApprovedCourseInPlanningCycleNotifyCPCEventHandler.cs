using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseInPlanningCycleNotifyCPCEventHandler : BaseTodoEventHandler<ApprovedCourseInPlanningCycleNotifyCPCEvent, ApprovedCourseInPlanningCycleNotifyCPCPayload>
    {
        public ApprovedCourseInPlanningCycleNotifyCPCEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override ApprovedCourseInPlanningCycleNotifyCPCPayload GetPayload(ApprovedCourseInPlanningCycleNotifyCPCEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(ApprovedCourseInPlanningCycleNotifyCPCEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
