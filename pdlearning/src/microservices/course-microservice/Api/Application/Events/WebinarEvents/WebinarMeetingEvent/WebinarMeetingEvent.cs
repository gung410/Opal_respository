using System.Diagnostics.CodeAnalysis;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events.WebinarEvents.WebinarMeetingEvent
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class WebinarMeetingEvent : BaseThunderEvent, IMQMessage
    {
        public WebinarMeetingEvent(WebinarMeetingRequest request, WebinarMeetingAction action)
        {
            BookMeetingRequest = request;
            BookMeetingAction = action;
        }

        public WebinarMeetingRequest BookMeetingRequest { get; }

        public WebinarMeetingAction BookMeetingAction { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.webinar.{BookMeetingAction.ToString().ToLower()}-meeting";
        }
    }
}
