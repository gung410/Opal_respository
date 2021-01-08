using System;

namespace Microservice.Webinar.Application.RequestDtos
{
    public class CheckMeetingAvailableRequest
    {
        public Guid MeetingId { get; set; }

        public string BBBServerPrivateIp { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
