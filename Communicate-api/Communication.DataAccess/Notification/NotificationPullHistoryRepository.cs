using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Communication.DataAccess.Notification
{
    public class NotificationPullHistoryRepository : RepositoryBase<NotificationPullHistory>, INotificationPullHistoryRepository
    {
        public NotificationPullHistoryRepository(IConfiguration configuration, IMongoDatabase database) : base(database, configuration)
        {
        }
        public async Task ReplaceOneAsync(NotificationPullHistory notificationPullHistory)
        {
            await _collection.ReplaceOneAsync(filter: new BsonDocument("_id", notificationPullHistory.UserId),
                                              options: new ReplaceOptions { IsUpsert = true },
                                              replacement: notificationPullHistory);
        }
    }
}