using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class RejectedOfferNotifyAdministratorEventHandler : BaseTodoEventHandler<RejectedOfferNotifyAdministratorEvent, RejectedOfferNotifyAdministratorPayload>
    {
        public RejectedOfferNotifyAdministratorEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override RejectedOfferNotifyAdministratorPayload GetPayload(RejectedOfferNotifyAdministratorEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(RejectedOfferNotifyAdministratorEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
