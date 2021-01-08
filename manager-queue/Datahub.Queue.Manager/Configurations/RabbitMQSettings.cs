namespace Datahub.Queue.Manager.Configurations
{
    public class RabbitMQSettings
    {
        public string[] HostNames { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool SslEnabled { get; set; }
    }
}
