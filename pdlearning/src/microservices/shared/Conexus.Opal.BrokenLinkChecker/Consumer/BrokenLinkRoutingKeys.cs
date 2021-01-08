namespace Conexus.Opal.BrokenLinkChecker
{
    public class BrokenLinkRoutingKeys
    {
        public const string EnqueueUrlsRoutingKey = "microservice.events.brokenlink.enqueue-urls";

        public const string DequeueObjectRoutingKey = "microservice.events.brokenlink.dequeue-object";
    }
}
