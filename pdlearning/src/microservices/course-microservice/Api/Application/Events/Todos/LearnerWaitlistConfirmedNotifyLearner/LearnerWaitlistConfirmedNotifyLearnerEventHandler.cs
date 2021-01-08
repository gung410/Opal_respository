using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class LearnerWaitlistConfirmedNotifyLearnerEventHandler : BaseTodoEventHandler<LearnerWaitlistConfirmedNotifyLearnerEvent, LearnerWaitlistConfirmedNotifyLearnerPayload>
    {
        public LearnerWaitlistConfirmedNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override LearnerWaitlistConfirmedNotifyLearnerPayload GetPayload(LearnerWaitlistConfirmedNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(LearnerWaitlistConfirmedNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
