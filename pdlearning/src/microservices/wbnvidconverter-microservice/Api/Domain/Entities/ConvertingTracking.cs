using System;
using System.ComponentModel.DataAnnotations;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.WebinarVideoConverter.Domain.Entities
{
    public class ConvertingTracking : AuditedEntity
    {
        public Guid MeetingId { get; set; }

        [Required]
        [MaxLength(54)]
        public string InternalMeetingId { get; set; }

        [ConcurrencyCheck]
        public ConvertStatus Status { get; set; }

        public FailStep FailedAtStep { get; set; }

        public int RetryCount { get; set; }

        public string S3Path { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
