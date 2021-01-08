using System;

namespace Microservice.Webinar.Application.Consumers.Messages
{
    public class UpdateMeetingPrivateIpMessage
    {
        public Guid MeetingId { get; set; }

        public string BBBServerPrivateIp { get; set; }
    }
}
