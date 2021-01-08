using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class SubmittedCourseContentNotifyApproverEventHandler : BaseTodoEventHandler<SubmittedCourseContentNotifyApproverEvent, SubmittedCourseContentNotifyApproverPayload>
    {
        public SubmittedCourseContentNotifyApproverEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetModule()
        {
            return "LMM";
        }

        protected override SubmittedCourseContentNotifyApproverPayload GetPayload(SubmittedCourseContentNotifyApproverEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(SubmittedCourseContentNotifyApproverEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
