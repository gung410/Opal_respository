using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentFeedbackNotifyLearnerEventHandler : BaseTodoEventHandler<AssignmentFeedbackNotifyLearnerEvent, AssignmentFeedbackNotifyLearnerPayload>
    {
        public AssignmentFeedbackNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetModule()
        {
            return "LMM";
        }

        protected override AssignmentFeedbackNotifyLearnerPayload GetPayload(AssignmentFeedbackNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(AssignmentFeedbackNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
