using System;

namespace Microservice.Webinar.Application.RequestDtos
{
    public class CreateMeetingRequest
    {
        public Guid MeetingId { get; set; }

        public string MeetingName { get; set; }

        public string BBBServerPrivateIp { get; set; }
    }
}
