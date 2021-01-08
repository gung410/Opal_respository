using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Conexus.Opal.OutboxPattern
{
    public interface IOutboxMessage : IAudited
    {
        /// <summary>
        /// The routing key of the message when sending to RabbitMQ.
        /// </summary>
        public string RoutingKey { get; set; }

        public string Exchange { get; set; }

        /// <summary>
        /// The message data was serialized to JSON, it will be sent to rabbitMq.
        /// </summary>
        public string MessageData { get; set; }

        public int SendTimes { get; set; }

        public string FailReason { get; set; }

        public string SourceIp { get; set; }

        public string UserId { get; set; }

        public MessageStatus Status { get; set; }

        /// <summary>
        /// Used to check when the message was prepared to be sent.
        /// </summary>
        public DateTime PreparedAt { get; set; }

        public bool ReadyToDelete { get; set; }
    }
}
