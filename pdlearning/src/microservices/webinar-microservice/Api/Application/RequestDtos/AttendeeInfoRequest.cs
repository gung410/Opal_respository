using System;

namespace Microservice.Webinar.Application.RequestDtos
{
    public class AttendeeInfoRequest
    {
        public Guid Id { get; set; }

        public bool IsModerator { get; set; }
    }
}
