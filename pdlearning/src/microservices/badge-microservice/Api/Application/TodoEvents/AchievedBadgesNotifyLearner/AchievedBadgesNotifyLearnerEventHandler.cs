using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Badge.Application.TodoEvents
{
    public class AchievedBadgesNotifyLearnerEventHandler : BaseTodoEventHandler<AchievedBadgesNotifyLearnerEvent, AchievedBadgesNotifyLearnerPayload>
    {
        public AchievedBadgesNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override AchievedBadgesNotifyLearnerPayload GetPayload(AchievedBadgesNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(AchievedBadgesNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
