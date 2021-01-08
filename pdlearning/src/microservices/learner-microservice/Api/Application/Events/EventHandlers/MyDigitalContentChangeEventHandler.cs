using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Learner.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Learner.Application.Events.EventHandlers
{
    public class MyDigitalContentChangeEventHandler : OutboxOpalMqEventHandler<MyDigitalContentChangeEvent, MyDigitalContent>
    {
        public MyDigitalContentChangeEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue outboxQueue) : base(options, userContext, outboxQueue)
        {
        }

        protected override MyDigitalContent GetMQMessagePayload(MyDigitalContentChangeEvent @event)
        {
            return @event.MyDigitalContent;
        }
    }
}
