using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Webinar.Domain.Entities
{
    public class Attendee : AuditedEntity
    {
        public Attendee()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid MeetingId { get; set; }

        public Guid UserId { get; set; }

        public bool IsModerator { get; set; }
    }
}
