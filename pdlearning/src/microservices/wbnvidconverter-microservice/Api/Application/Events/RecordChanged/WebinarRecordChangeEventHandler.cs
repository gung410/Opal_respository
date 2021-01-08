using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.WebinarVideoConverter.Application.RequestDtos;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarVideoConverter.Application.Events
{
    public class WebinarRecordChangeEventHandler : ScopedOutboxOpalMqEventHandler<WebinarRecordChangeEvent, WebinarRecordChangeRequest>
    {
        public WebinarRecordChangeEventHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IOptions<RabbitMQOptions> options,
            IUserContext userContext,
            IOutboxQueue outboxQueue) : base(unitOfWorkManager, options, userContext, outboxQueue)
        {
        }

        protected override WebinarRecordChangeRequest GetMQMessagePayload(WebinarRecordChangeEvent @event)
        {
            return @event.Model;
        }
    }
}
