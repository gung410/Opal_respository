using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssessmentAnswerCompletedNotifyLearnerEventHandler : BaseTodoEventHandler<AssessmentAnswerCompletedNotifyLearnerEvent, AssessmentAnswerCompletedNotifyLearnerPayload>
    {
        public AssessmentAnswerCompletedNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override AssessmentAnswerCompletedNotifyLearnerPayload GetPayload(AssessmentAnswerCompletedNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(AssessmentAnswerCompletedNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }

        protected override string GetModule()
        {
            return "LMM";
        }
    }
}
