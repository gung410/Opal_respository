using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Conexus.Opal.OutboxPattern
{
    public class OutboxMessage : AuditedEntity, IOutboxMessage
    {
        /// <inheritdoc/>
        public string RoutingKey { get; set; }

        public string Exchange { get; set; }

        /// <inheritdoc/>
        public string MessageData { get; set; }

        public int SendTimes { get; set; }

        public string FailReason { get; set; }

        public string SourceIp { get; set; }

        public string UserId { get; set; }

        public MessageStatus Status { get; set; }

        /// <inheritdoc/>
        public DateTime PreparedAt { get; set; }

        public byte[] Timestamp { get; set; }

        public bool ReadyToDelete { get; set; }
    }
}
