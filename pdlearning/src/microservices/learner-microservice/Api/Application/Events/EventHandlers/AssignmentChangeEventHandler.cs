using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Learner.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Learner.Application.Events.EventHandlers
{
    public class AssignmentChangeEventHandler : OutboxOpalMqEventHandler<AssignmentChangeEvent, MyAssignment>
    {
        public AssignmentChangeEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue outboxQueue)
            : base(options, userContext, outboxQueue)
        {
        }

        protected override MyAssignment GetMQMessagePayload(AssignmentChangeEvent @event)
        {
            return @event.Assignment;
        }
    }
}
