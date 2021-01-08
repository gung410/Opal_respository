using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class CoursePlanningCycleChangeEventHandler : OutboxOpalMqEventHandler<CoursePlanningCycleChangeEvent, CoursePlanningCycle>
    {
        public CoursePlanningCycleChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override CoursePlanningCycle GetMQMessagePayload(CoursePlanningCycleChangeEvent @event)
        {
            return @event.CoursePlanningCycle;
        }
    }
}
