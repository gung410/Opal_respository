using System;
using System.Collections.Generic;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Domain.Enums;

namespace Microservice.Webinar.Application.Consumers.Messages
{
    public class BookMeetingMessage
    {
        /// <summary>
        /// SessionId of the ClassRun(CAM) OR SessionId of the Meeting(CSL).
        /// </summary>
        public Guid SessionId { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// Relative s3 path.
        /// </summary>
        public string PreRecordPath { get; set; }

        public Guid? PreRecordId { get; set; }

        public IEnumerable<AttendeeInfoRequest> Attendees { get; set; }

        /// <summary>
        /// The meeting of Course(CAM) OR Community(CSL).
        /// </summary>
        public BookingSource Source { get; set; }
    }
}
