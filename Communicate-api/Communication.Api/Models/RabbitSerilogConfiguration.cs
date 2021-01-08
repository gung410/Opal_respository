namespace Communication.Api.Models
{
    public class RabbitSerilogConfiguration
    {
        public string Uri { get; set; }
        public string Exchange { get; set; }
        public string RouteKey { get; set; }
    }
}