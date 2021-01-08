using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.AssociatedEntities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class ClassRunChangeEventHandler : OutboxOpalMqEventHandler<ClassRunChangeEvent, ClassRunAssociatedEntity>
    {
        public ClassRunChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override ClassRunAssociatedEntity GetMQMessagePayload(ClassRunChangeEvent @event)
        {
            return @event.ClassRun;
        }
    }
}
