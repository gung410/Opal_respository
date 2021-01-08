using System;
using System.Collections.Generic;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class UpdateCommunityEventRequest
    {
        public Guid Id { get; set; }

        public CalendarEventSource CalendarEventSource { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public bool IsAllDay { get; set; }

        public CommunityEventPrivacy? CommunityEventPrivacy { get; set; }

        public DateTime? RepeatUntil { get; set; }

        public RepeatFrequency RepeatFrequency { get; set; }
    }
}
