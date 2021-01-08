using MongoDB.Driver;

namespace Conexus.Opal.Shared.MongoDb
{
    public interface IMongoDbContext
    {
        public IMongoDatabase Database { get; }
    }
}
