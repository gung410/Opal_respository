using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class ChangeClassSubmittedByLearnerNotifyApproverEventHandler : BaseTodoEventHandler<ChangeClassSubmittedByLearnerNotifyApproverEvent, ChangeClassSubmittedByLearnerNotifyApproverPayload>
    {
        public ChangeClassSubmittedByLearnerNotifyApproverEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override ChangeClassSubmittedByLearnerNotifyApproverPayload GetPayload(ChangeClassSubmittedByLearnerNotifyApproverEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(ChangeClassSubmittedByLearnerNotifyApproverEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
