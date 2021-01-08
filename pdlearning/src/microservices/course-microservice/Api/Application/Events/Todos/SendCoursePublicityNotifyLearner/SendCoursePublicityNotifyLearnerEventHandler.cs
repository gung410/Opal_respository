using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendCoursePublicityNotifyLearnerEventHandler : BaseTodoEventHandler<SendCoursePublicityNotifyLearnerEvent, SendCoursePublicityNotifyLearnerPayload>
    {
        public SendCoursePublicityNotifyLearnerEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override string GetMessagePrimary(SendCoursePublicityNotifyLearnerEvent @event)
        {
            return @event.Message;
        }

        protected override string GetPlainTextPrimary(SendCoursePublicityNotifyLearnerEvent @event)
        {
            return @event.PlainText;
        }

        protected override SendCoursePublicityNotifyLearnerPayload GetPayload(SendCoursePublicityNotifyLearnerEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(SendCoursePublicityNotifyLearnerEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
