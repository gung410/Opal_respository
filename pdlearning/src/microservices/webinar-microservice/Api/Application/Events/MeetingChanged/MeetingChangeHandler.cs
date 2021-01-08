using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Webinar.Application.Events;
using Microservice.Webinar.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Calendar.Application.Events.WebinarBooking
{
    public class WebinarMeetingEventHandler : OutboxOpalMqEventHandler<MeetingChangeEvent, MeetingChangeModel>
    {
        public WebinarMeetingEventHandler(IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue queue) : base(options, userContext, queue)
        {
        }

        protected override MeetingChangeModel GetMQMessagePayload(MeetingChangeEvent @event)
        {
            return @event.MeetingInfoRequest;
        }
    }
}
