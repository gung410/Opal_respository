using System;

namespace Microservice.Webinar.Application.Models
{
    public class AttendeeModel
    {
        public Guid MeetingId { get; set; }

        public Guid UserId { get; set; }

        public bool IsModerator { get; set; }
    }
}
