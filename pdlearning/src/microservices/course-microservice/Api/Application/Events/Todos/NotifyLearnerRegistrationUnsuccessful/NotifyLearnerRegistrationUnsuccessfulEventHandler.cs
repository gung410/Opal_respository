using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Application.Events.Todos
{
    public class NotifyLearnerRegistrationUnsuccessfulEventHandler : BaseTodoEventHandler<NotifyLearnerRegistrationUnsuccessfulEvent, NotifyLearnerRegistrationUnsuccessfulPayload>
    {
        public NotifyLearnerRegistrationUnsuccessfulEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue) : base(options, queue)
        {
        }

        protected override NotifyLearnerRegistrationUnsuccessfulPayload GetPayload(NotifyLearnerRegistrationUnsuccessfulEvent @event)
        {
            return @event.Payload;
        }

        protected override List<ReceiverDto> GetPrimary(NotifyLearnerRegistrationUnsuccessfulEvent @event)
        {
            return @event.AssignedToIds.Select(p => new ReceiverDto() { UserId = p.ToString() }).ToList();
        }
    }
}
