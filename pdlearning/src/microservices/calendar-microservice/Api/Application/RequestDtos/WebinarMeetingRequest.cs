using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.WebinarBooking;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class WebinarMeetingRequest
    {
        public Guid SessionId { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<AttendeeInfoRequest> Attendees { get; set; }

        public WebinarBookingSource Source { get; set; }
    }
}
