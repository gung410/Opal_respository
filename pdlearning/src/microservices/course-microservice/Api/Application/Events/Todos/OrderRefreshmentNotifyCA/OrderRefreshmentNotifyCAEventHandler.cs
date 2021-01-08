using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class OrderRefreshmentNotifyCAEventHandler : BaseTodoEventHandler<OrderRefreshmentNotifyCAEvent, OrderRefreshmentNotifyCAPayload>
    {
        public OrderRefreshmentNotifyCAEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override OrderRefreshmentNotifyCAPayload GetPayload(OrderRefreshmentNotifyCAEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(OrderRefreshmentNotifyCAEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
