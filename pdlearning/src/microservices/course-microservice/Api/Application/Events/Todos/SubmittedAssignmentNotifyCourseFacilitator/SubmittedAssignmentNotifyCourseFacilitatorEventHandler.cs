using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class SubmittedAssignmentNotifyCourseFacilitatorEventHandler : BaseTodoEventHandler<SubmittedAssignmentNotifyCourseFacilitatorEvent, SubmittedAssignmentNotifyCourseFacilitatorPayload>
    {
        public SubmittedAssignmentNotifyCourseFacilitatorEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetModule()
        {
            return "LMM";
        }

        protected override SubmittedAssignmentNotifyCourseFacilitatorPayload GetPayload(SubmittedAssignmentNotifyCourseFacilitatorEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(SubmittedAssignmentNotifyCourseFacilitatorEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
