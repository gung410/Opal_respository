using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssignedAssignmentNotifyLearnerEventHandler : BaseTodoEventHandler<AssignedAssignmentNotifyLearnerEvent, AssignedAssignmentNotifyLearnerPayload>
    {
        public AssignedAssignmentNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetModule()
        {
            return "LMM";
        }

        protected override AssignedAssignmentNotifyLearnerPayload GetPayload(AssignedAssignmentNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(AssignedAssignmentNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
