using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Models
{
    public class EventModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public CalendarEventSource Source { get; set; }

        public Guid? SourceId { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime? RepeatUntil { get; set; }

        public bool IsAllDay { get; set; }

        public string Description { get; set; }

        public EventType Type { get; set; }

        public RepeatFrequency RepeatFrequency { get; set; }
    }
}
