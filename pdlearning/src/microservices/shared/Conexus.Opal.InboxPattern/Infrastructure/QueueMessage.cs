using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Conexus.Opal.InboxPattern.Infrastructure
{
    public class QueueMessage
    {
        public QueueMessage(Guid messageId, IMQMessage message, DateTime created)
        {
            MessageId = messageId;
            Message = message;
            Created = created;
        }

        public Guid MessageId { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// The message data with actual type is <seealso cref="IMQMessage"/>.
        /// This property was set as <see cref="object"/>
        /// in order to support <c>JsonSerializer.Serialize</c> method
        /// with <c>ThunderJsonSerializerOptions.SharedJsonSerializerOptions</c> option.
        /// </summary>
        public object Message { get; private set; }
    }
}
