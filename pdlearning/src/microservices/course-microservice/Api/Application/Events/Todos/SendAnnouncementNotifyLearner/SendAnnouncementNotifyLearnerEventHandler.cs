using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendAnnouncementNotifyLearnerEventHandler : BaseTodoEventHandler<SendAnnouncementNotifyLearnerEvent, SendAnnouncementNotifyLearnerPayload>
    {
        public SendAnnouncementNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetMessagePrimary(SendAnnouncementNotifyLearnerEvent @event)
        {
            return @event.Message;
        }

        protected override string GetPlainTextPrimary(SendAnnouncementNotifyLearnerEvent @event)
        {
            return @event.PlainText;
        }

        protected override SendAnnouncementNotifyLearnerPayload GetPayload(SendAnnouncementNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(SendAnnouncementNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
