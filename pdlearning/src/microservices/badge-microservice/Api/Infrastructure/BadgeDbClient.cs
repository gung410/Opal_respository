using Conexus.Opal.Shared.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Microservice.Badge.Infrastructure
{
    public class BadgeDbClient
    {
        public BadgeDbClient(IOptions<MongoOptions> options)
        {
            var url = MongoUrl.Create(options.Value.ConnectionString);
            var mongoSettings = MongoClientSettings.FromUrl(url);
            MongoClient = new MongoClient(mongoSettings);
        }

        public MongoClient MongoClient { get; }
    }
}
