using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Calendar.Application.RequestDtos;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Calendar.Application.Events.WebinarBooking
{
    public class WebinarMeetingEventHandler : OutboxOpalMqEventHandler<WebinarMeetingEvent, WebinarMeetingRequest>
    {
        public WebinarMeetingEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue queue) : base(options, userContext, queue)
        {
        }

        protected override WebinarMeetingRequest GetMQMessagePayload(WebinarMeetingEvent @event)
        {
            return @event.Model;
        }
    }
}
