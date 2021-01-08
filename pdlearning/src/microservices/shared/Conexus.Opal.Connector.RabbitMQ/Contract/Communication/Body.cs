namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public class Body
    {
        public Recipient Recipient { get; set; }

        public Channel? Channel { get; set; }

        public Message Message { get; set; }

        public bool IsHtmlEmail { get; set; }

        public TemplateData TemplateData { get; set; }
    }
}
