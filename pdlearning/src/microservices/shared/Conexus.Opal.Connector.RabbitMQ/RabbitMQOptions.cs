using System.Collections.Generic;

namespace Conexus.Opal.Connector.RabbitMQ
{
    public class RabbitMQOptions
    {
        public string HostNames { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Define the queue to listen to.
        /// Combo: QueueName, ExchangeName and RoutingKey need to be declared.
        /// An EXCHANGE will bind to a QUEUE with a ROUTING KEY.
        /// </summary>
        public string QueueName { get; set; }

        public string ExchangeName { get; set; }

        public List<string> RoutingKeys { get; set; }

        public int RetryCount { get; set; }

        public bool SslEnabled { get; set; }

        public string DefaultCommandExchange { get; set; }

        public string DefaultEventExchange { get; set; }

        public string DefaultActivityExchange { get; set; }

        public string DefaultAuditLogExchange { get; set; }

        public string DefaultIntegrationExchange { get; set; }
    }
}
