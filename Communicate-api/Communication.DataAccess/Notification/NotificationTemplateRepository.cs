using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.DataAccess.Notification
{
    public class NotificationTemplateRepository : RepositoryBase<NotificationTemplate>,INotificationTemplateRepository
    {
        public NotificationTemplateRepository(IConfiguration configuration, IMongoDatabase database) : base(database, configuration)
        {

        }
    }
}
