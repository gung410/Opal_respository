using System;

namespace LearnerApp.Models.Calendar
{
    public class CalendarEvent
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public EventSource Source { get; set; }

        public string SourceId { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndtAt { get; set; }

        public DateTime RepeatUntil { get; set; }

        public bool IsAllDay { get; set; }

        public string Description { get; set; }

        public EventType Type { get; set; }

        public EventRepeatFrequency RepeatFrequency { get; set; }
    }
}
