using System;
using System.ComponentModel.DataAnnotations;
using Thunder.Platform.Core.Domain.Auditing;

namespace Conexus.Opal.InboxPattern.Entities
{
    public class InboxMessage : AuditedEntity
    {
        /// <summary>
        /// The message data was serialized to JSON, it will be sent to rabbitMq.
        /// </summary>
        public string MessageData { get; set; }

        public Guid MessageId { get; set; }

        public DateTime MessageCreatedAt { get; set; }

        [ConcurrencyCheck]
        public MessageStatus Status { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        [ConcurrencyCheck]
        public bool ReadyToDelete { get; set; }
    }
}
