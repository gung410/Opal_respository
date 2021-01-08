namespace Datahub.Queue.Manager.Test.MongoDbProcessor
{
    internal class QueueConfiguration
    {
        public string Exchange { get; set; }
        public string ExchangeType { get; set; }
        public string Queue { get; set; }
        public string RoutingKey { get; set; }
    }
}