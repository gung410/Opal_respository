using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseMLUNotifyCollaboratorEventHandler : BaseTodoEventHandler<ApprovedCourseMLUNotifyCollaboratorEvent, ApprovedCourseMLUNotifyCollaboratorPayload>
    {
        public ApprovedCourseMLUNotifyCollaboratorEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override ApprovedCourseMLUNotifyCollaboratorPayload GetPayload(ApprovedCourseMLUNotifyCollaboratorEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(ApprovedCourseMLUNotifyCollaboratorEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
