using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Conexus.Opal.OutboxPattern
{
    public class QueueMessage
    {
        public QueueMessage(string routingKey, IMQMessage message)
        {
            RoutingKey = routingKey;
            Message = message;
        }

        public QueueMessage(string routingKey, IMQMessage message, string exchange)
        {
            RoutingKey = routingKey;
            Message = message;
            Exchange = exchange;
        }

        public string RoutingKey { get; set; }

        public string Exchange { get; set; }

        /// <summary>
        /// The message data with actual type is <seealso cref="IMQMessage"/>.
        /// This property was set as <see cref="object"/>
        /// in order to support <c>JsonSerializer.Serialize</c> method
        /// with <c>ThunderJsonSerializerOptions.SharedJsonSerializerOptions</c> option.
        /// </summary>
        public object Message { get; private set; }
    }
}
