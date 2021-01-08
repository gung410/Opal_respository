using System;
using Microservice.WebinarVideoConverter.Domain.Enums;

namespace Microservice.WebinarVideoConverter.Application.RequestDtos
{
    public class WebinarRecordChangeRequest
    {
        public Guid MeetingId { get; set; }

        public Guid RecordId { get; set; }

        public string InternalMeetingId { get; set; }

        public string VideoPath { get; set; }

        public string Extension { get; set; } = "mp4";

        public double FileSize { get; set; }

        public ConvertStatus Status { get; set; }
    }
}
