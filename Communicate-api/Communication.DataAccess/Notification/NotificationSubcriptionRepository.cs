using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Communication.DataAccess.Notification
{
    public class NotificationSubcriptionRepository : RepositoryBase<NotificationSubcription>, INotificationSubcriptionRepository
    {

        public NotificationSubcriptionRepository(IConfiguration configuration, IMongoDatabase database) : base(database,configuration)
        {

        }

    }
}
