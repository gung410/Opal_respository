using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Communication.DataAccess.Notification
{
    public class NotificationHistoryRepository : RepositoryBase<NotificationHistory>, INotificationHistoryRepository
    {
     
        public NotificationHistoryRepository(IConfiguration configuration, IMongoDatabase database) : base(database, configuration)
        {
          
        }
       
    }
}
