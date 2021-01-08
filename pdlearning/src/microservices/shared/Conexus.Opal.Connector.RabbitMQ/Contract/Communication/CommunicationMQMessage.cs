namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public enum Channel
    {
        Default,
        Email,
        SMS,
        System
    }

    public class CommunicationMQMessage : OpalMQMessage<Body>
    {
        public CommunicationMQMessage()
        {
        }

        public string Version { get; set; }
    }
}
