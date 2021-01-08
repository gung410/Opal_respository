using System;
using Conexus.Opal.Shared.MongoDb;
using Thunder.Platform.Core.Timing;

namespace Conexus.Opal.OutboxPattern.Variants.MongoDb.Entities
{
    public class MongoOutboxMessage : MongoEntity, IOutboxMessage
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

        public bool ReadyToDelete { get; set; }

        public DateTime CreatedDate { get; set; } = Clock.Now;

        public DateTime? ChangedDate { get; set; }

        public int Version { get; set; } = 1;
    }
}
