using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Events.WebinarBooking
{
    public class WebinarMeetingEvent : BaseThunderEvent, IMQMessage
    {
        public WebinarMeetingEvent(WebinarMeetingRequest request, WebinarMeetingAction action)
        {
            Model = request;
            WebinarMeetingAction = action;
        }

        public WebinarMeetingRequest Model { get; }

        public WebinarMeetingAction WebinarMeetingAction { get; }

        public override string GetRoutingKey() => $"microservice.events.webinar.{WebinarMeetingAction.ToString().ToLower()}-meeting";
    }
}
