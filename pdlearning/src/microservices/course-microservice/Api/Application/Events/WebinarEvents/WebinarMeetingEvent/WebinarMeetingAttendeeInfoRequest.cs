using System;

namespace Microservice.Course.Application.Events.WebinarEvents.WebinarMeetingEvent
{
    public class WebinarMeetingAttendeeInfoRequest
    {
        public Guid Id { get; set; }

        public bool IsModerator { get; set; }
    }
}
