using System;
using Microservice.Webinar.Domain.Enums;

namespace Microservice.Webinar.Application.Consumers
{
    public class WebinarRecordChangedMessage
    {
        public Guid MeetingId { get; set; }

        public Guid RecordId { get; set; }

        public string InternalMeetingId { get; set; }

        public string VideoPath { get; set; }

        public string Extension { get; set; }

        public double FileSize { get; set; }

        public RecordStatus Status { get; set; }
    }
}
