using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Communication.DataAccess.Notification
{
    public class NotificationTemplateLanguageRepository : RepositoryBase<NotificationTemplateLanguage>, INotificationTemplateLanguageRepository
    {
        public NotificationTemplateLanguageRepository(IConfiguration configuration, IMongoDatabase database) : base(database, configuration)
        {
        }
    }
}
