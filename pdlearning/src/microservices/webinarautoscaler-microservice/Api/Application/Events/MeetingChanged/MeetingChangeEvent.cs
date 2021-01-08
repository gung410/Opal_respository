using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.WebinarAutoscaler.Application.RequestDtos;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Events
{
    public class MeetingChangeEvent : BaseThunderEvent, IMQMessage
    {
        public MeetingChangeEvent(MeetingInfoRequest meetingInfo, MeetingChangeType changeType)
        {
            MeetingInfo = meetingInfo;
            ChangeType = changeType;
        }

        public MeetingInfoRequest MeetingInfo { get; set; }

        public MeetingChangeType ChangeType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.webinarautoscaler.bbb-server-private-ip-{ChangeType.ToString().ToLower()}";
        }
    }
}
