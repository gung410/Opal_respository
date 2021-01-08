using Conexus.Opal.OutboxPattern.Variants.MongoDb.Entities;
using Conexus.Opal.Shared.MongoDb;
using MongoDB.Driver;

namespace Conexus.Opal.OutboxPattern.Variants.MongoDb.Infrastructure
{
    public interface IHasOutboxCollection : IMongoDbContext
    {
        public IMongoCollection<MongoOutboxMessage> OutboxMessageCollection => Database.GetCollection<MongoOutboxMessage>("OutboxMessages");
    }
}
