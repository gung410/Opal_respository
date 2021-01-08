using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class AssessmentAnswerAssignedNotifyAssigneeEventHandler : BaseTodoEventHandler<AssessmentAnswerAssignedNotifyAssigneeEvent, AssessmentAnswerAssignedNotifyAssigneePayload>
    {
        public AssessmentAnswerAssignedNotifyAssigneeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override AssessmentAnswerAssignedNotifyAssigneePayload GetPayload(AssessmentAnswerAssignedNotifyAssigneeEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(AssessmentAnswerAssignedNotifyAssigneeEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }

        protected override string GetModule()
        {
            return "LMM";
        }
    }
}
