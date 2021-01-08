using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Microservice.NewsFeed.Infrastructure
{
    public class NewsFeedDbClient
    {
        public NewsFeedDbClient(IOptions<MongoOptions> options)
        {
            var url = MongoUrl.Create(options.Value.ConnectionString);
            MongoClient = new MongoClient(url);
        }

        public MongoClient MongoClient { get; }
    }
}
