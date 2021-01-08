namespace Datahub.Queue.Manager.Test.Processor
{
    internal class RabbitMQSettings
    {
        public string[] HostNames { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueManagerAPI { get; internal set; }
    }
}