using System;
using Microservice.WebinarVideoConverter.Domain.Enums;

namespace Microservice.WebinarVideoConverter.Application.Models
{
    public class ConvertingTrackingModel
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }

        public string InternalMeetingId { get; set; }

        public ConvertStatus Status { get; set; }
    }
}
