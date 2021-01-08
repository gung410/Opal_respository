using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Communication.DataAccess.Notification
{
    public class NotificationReferenceRepository : RepositoryBase<NotificationUserInfo>, INotificationReferenceRepository
    {

        public NotificationReferenceRepository(IConfiguration configuration, IMongoDatabase database) : base(database, configuration)
        {
          
        }
       
    }
}
