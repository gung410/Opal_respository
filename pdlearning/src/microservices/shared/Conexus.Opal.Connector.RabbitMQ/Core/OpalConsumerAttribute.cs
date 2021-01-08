using System;

namespace Conexus.Opal.Connector.RabbitMQ.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OpalConsumerAttribute : Attribute
    {
        public OpalConsumerAttribute(string routingKey)
        {
            RoutingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));
        }

        /// <summary>
        /// Routing key name.
        /// </summary>
        public string RoutingKey { get; }
    }
}
