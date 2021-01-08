using System;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public class OpalMQMessage<TBody> : IMQMessage where TBody : class
    {
        public string Type { get; set; }

        /// <summary>
        /// Friendly name of an message. It could be the routing key.
        /// This is for filter in Kibana.
        /// </summary>
        public string Name { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Message Id.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        public OpalMQMessageRouting Routing { get; set; } = new OpalMQMessageRouting();

        public OpalMQMessagePayload<TBody> Payload { get; set; } = new OpalMQMessagePayload<TBody>();
    }
}
