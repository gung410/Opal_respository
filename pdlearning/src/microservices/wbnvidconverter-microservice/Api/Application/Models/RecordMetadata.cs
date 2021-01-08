using System;

namespace Microservice.WebinarVideoConverter.Application.Models
{
    public class RecordMetadata
    {
        public Guid MeetingId { get; set; }

        public string InternalMeetingId { get; set; }
    }
}
