using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Learner.Application.Events.Todo.ArchivedCourseNotification
{
    public class ArchivedCourseNotificationEventHandler : BaseTodoEventHandler<ArchivedCourseNotificationEvent, ArchivedCourseNotificationPayload>
    {
        public ArchivedCourseNotificationEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue outboxQueue) : base(options, outboxQueue)
        {
        }

        protected override ArchivedCourseNotificationPayload GetPayload(ArchivedCourseNotificationEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(ArchivedCourseNotificationEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto { UserId = p.ToString() }).ToList();
        }
    }
}
