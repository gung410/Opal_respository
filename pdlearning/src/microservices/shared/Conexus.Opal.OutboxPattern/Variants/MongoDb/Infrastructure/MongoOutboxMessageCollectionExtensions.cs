using System.Threading.Tasks;
using Conexus.Opal.OutboxPattern.Variants.MongoDb.Entities;
using MongoDB.Driver;

namespace Conexus.Opal.OutboxPattern.Variants.MongoDb.Infrastructure
{
    /// <summary>
    /// Update the document with represent version.
    /// </summary>
    internal static class MongoOutboxMessageCollectionExtensions
    {
        public static void UpdateWithIncreaseVersion(
            this IMongoCollection<MongoOutboxMessage> outboxMessageMongoCollection,
            MongoOutboxMessage outboxMessage)
        {
            var currentVersion = outboxMessage.Version;
            outboxMessage.Version += 1;

            outboxMessageMongoCollection.ReplaceOne(
                p => p.Id == outboxMessage.Id && p.Version == currentVersion,
                outboxMessage);
        }

        /// <summary>
        /// Update the document with represent version in async.
        /// </summary>
        /// <param name="outboxMessageMongoCollection">Outbox message collection.</param>
        /// <param name="outboxMessage">Outbox message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task UpdateWithIncreaseVersionAsync(
            this IMongoCollection<MongoOutboxMessage> outboxMessageMongoCollection,
            MongoOutboxMessage outboxMessage)
        {
            var currentVersion = outboxMessage.Version;
            outboxMessage.Version += 1;

            return outboxMessageMongoCollection.ReplaceOneAsync(
                p => p.Id == outboxMessage.Id && p.Version == currentVersion,
                outboxMessage);
        }
    }
}
