using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class NotifyContentBrokenLinkEventHandler : BaseTodoEventHandler<NotifyContentBrokenLinkEvent, NotifyContentBrokenLinkPayload>
    {
        public NotifyContentBrokenLinkEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetModule()
        {
            return "LMM";
        }

        protected override List<ReminderByDto> GetReminderByPrimary(NotifyContentBrokenLinkEvent @event)
        {
            return new List<ReminderByDto>()
            {
                 @event.ReminderByConditions
            };
        }

        protected override NotifyContentBrokenLinkPayload GetPayload(NotifyContentBrokenLinkEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(NotifyContentBrokenLinkEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
