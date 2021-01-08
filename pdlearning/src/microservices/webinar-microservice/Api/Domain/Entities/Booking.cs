using System;
using Microservice.Webinar.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Webinar.Domain.Entities
{
    public class Booking : AuditedEntity
    {
        public Booking()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid MeetingId { get; set; }

        public BookingSource Source { get; set; }

        public Guid SourceId { get; set; }
    }
}
