using System;

namespace Microservice.Webinar.Application.RequestDtos
{
    public class JoinMeetingRequest
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string MeetingId { get; set; }

        public bool IsModerator { get; set; }

        public bool Redirect { get; set; }

        public string BBBServerPrivateIp { get; set; }
    }
}
