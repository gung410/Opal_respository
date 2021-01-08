using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendCourseNominationAnnouncementNotifyEventHandler : BaseTodoEventHandler<SendCourseNominationAnnouncementNotifyEvent, SendCourseNominationAnnouncementNotifyPayload>
    {
        public SendCourseNominationAnnouncementNotifyEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetMessagePrimary(SendCourseNominationAnnouncementNotifyEvent @event)
        {
            return @event.Message;
        }

        protected override string GetPlainTextPrimary(SendCourseNominationAnnouncementNotifyEvent @event)
        {
            return @event.PlainText;
        }

        protected override SendCourseNominationAnnouncementNotifyPayload GetPayload(SendCourseNominationAnnouncementNotifyEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(SendCourseNominationAnnouncementNotifyEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
