using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class AssignmentChangeEventHandler : OutboxOpalMqEventHandler<AssignmentChangeEvent, AssignmentModel>
    {
        public AssignmentChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override AssignmentModel GetMQMessagePayload(AssignmentChangeEvent @event)
        {
            return @event.Assignment;
        }
    }
}
