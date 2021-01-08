using System;
using System.Linq;
using Communication.DataAccess.Notification;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Communication.DataAccess
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterDataAccess(this IServiceCollection serviceCollection, IConfiguration configuration, bool createIndex = true)
        {
            var _mogoUrl = new MongoUrl(configuration["MONGO_CONNECTIONSTRING"]);
            var clientSettings = MongoClientSettings.FromUrl(_mogoUrl);
            clientSettings.UseTls = bool.TryParse(Environment.GetEnvironmentVariable("MONGO_SSL_ENABLED"), out var sslEnabledValue) ? sslEnabledValue : false;
            clientSettings.AllowInsecureTls = false;
            var database = new MongoClient(clientSettings).GetDatabase(_mogoUrl.DatabaseName);
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
            serviceCollection.AddSingleton<IMongoDatabase>(database);
            if (createIndex)
            {
                var collectionUserNotificationSetting = database.GetCollection<UserNotificationSetting>(typeof(UserNotificationSetting).Name);
                CreateUserSettingIndexes(collectionUserNotificationSetting);
                var collectionNotificationHistory = database.GetCollection<NotificationHistory>("NotificationHistory");
                CreateNotificationHistoryIndexes(collectionNotificationHistory);
                var collectionNotification = database.GetCollection<Notification.Notification>("Notification");
                CreateNotificationIndexes(collectionNotification);
                CreateNotificationClientIdExternalIdIndexes(collectionNotification);
                CreateNotificationCreatedDateIndexes(collectionNotification);
                CreateNotificationStartDateEndDateTypeGlobalActiveIndexes(collectionNotification);
            }

            serviceCollection.AddScoped<INotificationHistoryRepository, NotificationHistoryRepository>();
            serviceCollection.AddScoped<INotificationRepository, NotificationRepository>();
            serviceCollection.AddScoped<INotificationSubcriptionRepository, NotificationSubcriptionRepository>();
            serviceCollection.AddScoped<INotificationReferenceRepository, NotificationReferenceRepository>();
            serviceCollection.AddScoped<IUserNotificationSettingRepository, UserNotificationSettingRepository>();
            serviceCollection.AddScoped<INotificationTemplateLanguageRepository, NotificationTemplateLanguageRepository>();
            serviceCollection.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
            serviceCollection.AddScoped<INotificationPullHistoryRepository, NotificationPullHistoryRepository>();
        }

        private static void CreateUserSettingIndexes(IMongoCollection<UserNotificationSetting> userNotificationSettingCollection)
        {
            var keys = Builders<UserNotificationSetting>.IndexKeys.Ascending(x => x.UserId);
            var options = new CreateIndexOptions
            {
                Name = "UserId",
                Unique = false,
                Sparse = false,
                Background = true
            };
            var indexList = userNotificationSettingCollection.Indexes.List().ToList();
            if (indexList.FirstOrDefault(x => x.GetElement("name").Value == "UserId") == null)
            {
                userNotificationSettingCollection.Indexes.CreateOne(new CreateIndexModel<UserNotificationSetting>(keys, options));
            }
        }
        private static void CreateNotificationHistoryIndexes(IMongoCollection<NotificationHistory> collection)
        {
            var keys = Builders<NotificationHistory>.IndexKeys
                .Ascending(x => x.UserId)
                .Ascending(x => x.NotificationId)
                .Ascending(x => x.DateReadUtc)
                .Ascending(x => x.CreatedDateUtc)
                .Ascending(x => x.DeletedDateUtc)
                .Ascending(x => x.NotificationType);
            var options = new CreateIndexOptions
            {
                Name = "NotificationHistory",
                Unique = false,
                Sparse = false,
                Background = true
            };
            var indexList = collection.Indexes.List().ToList();
            if (indexList.FirstOrDefault(x => x.GetElement("name").Value == "NotificationHistory") == null)
            {
                collection.Indexes.CreateOne(new CreateIndexModel<NotificationHistory>(keys, options));
            }
        }
        private static void CreateNotificationIndexes(IMongoCollection<Communication.DataAccess.Notification.Notification> collection)
        {
            var keys = Builders<Communication.DataAccess.Notification.Notification>.IndexKeys
                 .Ascending(x => x.IsGlobal)
                 .Ascending(x => x.Active)
                 .Ascending(x => x.StartDateUtc)
                 .Ascending(x => x.EndDateUtc)
                 .Ascending(x => x.NotificationType)
                 .Ascending(x => x.CreatedDateUtc);
            var options = new CreateIndexOptions
            {
                Name = "Notification",
                Unique = false,
                Sparse = false,
                Background = true
            };
            var indexList = collection.Indexes.List().ToList();
            if (indexList.FirstOrDefault(x => x.GetElement("name").Value == "Notification") == null)
            {
                collection.Indexes.CreateOne(new CreateIndexModel<Communication.DataAccess.Notification.Notification>(keys, options));
            }
        }

        private static void CreateNotificationClientIdExternalIdIndexes(IMongoCollection<Communication.DataAccess.Notification.Notification> collection)
        {
            var keys = Builders<Communication.DataAccess.Notification.Notification>.IndexKeys
                 .Ascending(x => x.ClientId)
                 .Ascending(x => x.ExternalId);
            var options = new CreateIndexOptions
            {
                Name = "Notification_ClientId_ExternalId",
                Unique = false,
                Sparse = false,
                Background = true
            };
            var indexList = collection.Indexes.List().ToList();
            if (indexList.FirstOrDefault(x => x.GetElement("name").Value == "Notification_ClientId_ExternalId") == null)
            {
                collection.Indexes.CreateOne(new CreateIndexModel<Communication.DataAccess.Notification.Notification>(keys, options));
            }
        }
        private static void CreateNotificationCreatedDateIndexes(IMongoCollection<Communication.DataAccess.Notification.Notification> collection)
        {
            var keys = Builders<Communication.DataAccess.Notification.Notification>.IndexKeys
                 .Ascending(x => x.CreatedDateUtc)
                 .Ascending(x => x.ExternalId);
            var options = new CreateIndexOptions
            {
                Name = "Notification_CreatedDateUtc",
                Unique = false,
                Sparse = false,
                Background = true
            };
            var indexList = collection.Indexes.List().ToList();
            if (indexList.FirstOrDefault(x => x.GetElement("name").Value == "Notification_CreatedDateUtc") == null)
            {
                collection.Indexes.CreateOne(new CreateIndexModel<Communication.DataAccess.Notification.Notification>(keys, options));
            }
        }

        private static void CreateNotificationStartDateEndDateTypeGlobalActiveIndexes(IMongoCollection<Communication.DataAccess.Notification.Notification> collection)
        {
            var keys = Builders<Communication.DataAccess.Notification.Notification>.IndexKeys
                 .Ascending(x => x.StartDateUtc)
                 .Ascending(x => x.EndDateUtc)
                 .Ascending(x => x.NotificationType)
                 .Ascending(x => x.IsGlobal)
                 .Ascending(x => x.Active);
            var options = new CreateIndexOptions
            {
                Name = "Notification_StartDate_EndDate_Type",
                Unique = false,
                Sparse = false,
                Background = true
            };
            var indexList = collection.Indexes.List().ToList();
            if (indexList.FirstOrDefault(x => x.GetElement("name").Value == "Notification_StartDate_EndDate_Type") == null)
            {
                collection.Indexes.CreateOne(new CreateIndexModel<Communication.DataAccess.Notification.Notification>(keys, options));
            }
        }
    }
}
