using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Webinar.Application.RequestDtos;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Webinar.Application.Events.RecordChanged
{
    public class RecordChangeEventHandler : OutboxOpalMqEventHandler<RecordChangeEvent, SaveUploadedContentRequest>
    {
        public RecordChangeEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue queue) : base(options, userContext, queue)
        {
        }

        protected override SaveUploadedContentRequest GetMQMessagePayload(RecordChangeEvent @event)
        {
            return @event.Model;
        }
    }
}
