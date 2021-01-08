using System;
using System.Collections.Generic;
using Microservice.Webinar.Domain.Enums;

namespace Microservice.Webinar.Application.RequestDtos
{
    public class BookMeetingRequest
    {
        public Guid SessionId { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Relative s3 path.
        /// </summary>
        public string PreRecordPath { get; set; }

        public Guid? PreRecordId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<AttendeeInfoRequest> Attendees { get; set; }

        public BookingSource Source { get; set; }
    }
}
