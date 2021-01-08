namespace Microservice.NewsFeed.Infrastructure
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        public string DatabaseLatestVersion { get; set; }
    }
}
