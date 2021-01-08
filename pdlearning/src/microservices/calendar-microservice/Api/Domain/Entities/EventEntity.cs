using System;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Calendar.Domain.Entities
{
    public class EventEntity : FullAuditedEntity
    {
        public EventEntity()
        {
            this.Id = Guid.NewGuid();
        }

        public EventEntity(Guid id)
        {
            this.Id = id != Guid.Empty ? id : Guid.NewGuid();
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid? CreatedBy { get; set; }

        public CalendarEventSource Source { get; set; }

        public Guid? SourceId { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public bool IsAllDay { get; set; }

        public EventType Type { get; set; }

        public Guid? SourceParentId { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Opening;

        public DateTime? RepeatUntil { get; set; }

        public RepeatFrequency RepeatFrequency { get; set; } = RepeatFrequency.None;
    }
}
