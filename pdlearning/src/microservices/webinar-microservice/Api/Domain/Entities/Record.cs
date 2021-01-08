using System;
using Microservice.Webinar.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Webinar.Domain.Entities
{
    public class Record : AuditedEntity
    {
        public Guid DigitalContentId { get; set; }

        public Guid MeetingId { get; set; }

        public string InternalMeetingId { get; set; }

        public Guid RecordId { get; set; }

        public RecordStatus Status { get; set; }
    }
}
