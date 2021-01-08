using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedRegistrationNotifyLearnerEventHandler : BaseTodoEventHandler<RejectedRegistrationNotifyLearnerEvent, RejectedRegistrationNotifyLearnerPayload>
    {
        public RejectedRegistrationNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override RejectedRegistrationNotifyLearnerPayload GetPayload(RejectedRegistrationNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(RejectedRegistrationNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
