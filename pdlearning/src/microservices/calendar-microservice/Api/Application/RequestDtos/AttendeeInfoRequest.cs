using System;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class AttendeeInfoRequest
    {
        public Guid Id { get; set; }

        public bool IsModerator { get; set; }
    }
}
