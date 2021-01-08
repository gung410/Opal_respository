using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendOrderRefreshmentNotifySpecificUsersEventHandler : BaseTodoEventHandler<SendOrderRefreshmentNotifySpecificUsersEvent, SendOrderRefreshmentNotifySpecificUsersPayload>
    {
        public SendOrderRefreshmentNotifySpecificUsersEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetMessagePrimary(SendOrderRefreshmentNotifySpecificUsersEvent @event)
        {
            return @event.Message;
        }

        protected override SendOrderRefreshmentNotifySpecificUsersPayload GetPayload(SendOrderRefreshmentNotifySpecificUsersEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(SendOrderRefreshmentNotifySpecificUsersEvent @event)
        {
            return @event.AssignedToEmails.Select(p => new ReceiverDto() { DirectEmail = p.ToString() }).ToList();
        }
    }
}
