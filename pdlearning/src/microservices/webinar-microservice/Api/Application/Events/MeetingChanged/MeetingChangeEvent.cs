using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Webinar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Events
{
    public class MeetingChangeEvent : BaseThunderEvent, IMQMessage
    {
        public MeetingChangeEvent(MeetingChangeModel meetingInfo, MeetingChangeType changeType)
        {
            MeetingInfoRequest = meetingInfo;
            ChangeType = changeType;
        }

        public MeetingChangeModel MeetingInfoRequest { get; set; }

        public MeetingChangeType ChangeType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.webinar.meeting-info.{ChangeType.ToString().ToLower()}";
        }
    }
}
