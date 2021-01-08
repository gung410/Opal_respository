using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Webinar.Application.Events;
using Microservice.WebinarAutoscaler.Application.RequestDtos;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Events.WebinarBooking
{
    public class WebinarMeetingEventHandler : ScopedOutboxOpalMqEventHandler<MeetingChangeEvent, MeetingInfoRequest>
    {
        public WebinarMeetingEventHandler(IUnitOfWorkManager unitOfWorkManager, IOptions<RabbitMQOptions> options, IUserContext userContext, IOutboxQueue queue) : base(unitOfWorkManager, options, userContext, queue)
        {
        }

        protected override MeetingInfoRequest GetMQMessagePayload(MeetingChangeEvent @event)
        {
            return @event.MeetingInfo;
        }
    }
}
