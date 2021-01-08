using System;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public class OpalMQMessageReferences
    {
        public OpalMQMessageReferences()
        {
        }

        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        public Guid? ExternalId { get; set; }

        public Guid? CommandId { get; set; }
    }
}
