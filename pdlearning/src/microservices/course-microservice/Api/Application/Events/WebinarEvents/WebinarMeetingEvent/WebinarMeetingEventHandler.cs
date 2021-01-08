using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events.WebinarEvents.WebinarMeetingEvent
{
    public class WebinarMeetingEventHandler : OpalMQEventHandler<WebinarMeetingEvent, WebinarMeetingRequest>
    {
        public WebinarMeetingEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer, IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override WebinarMeetingRequest GetMQMessagePayload(WebinarMeetingEvent @event)
        {
            return @event.BookMeetingRequest;
        }
    }
}
