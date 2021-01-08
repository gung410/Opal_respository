namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public class OpalMQMessagePayload<TBody> where TBody : class
    {
        public OpalMQMessageIdentity Identity { get; set; } = new OpalMQMessageIdentity();

        public OpalMQMessageReferences References { get; set; } = new OpalMQMessageReferences();

        public TBody Body { get; set; }
    }
}
