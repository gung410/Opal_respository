using Conexus.Opal.OutboxPattern.Variants.MongoDb.Infrastructure;
using Conexus.Opal.Shared.MongoDb;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Microservice.Badge.Infrastructure
{
    public class BadgeDbContext : IMongoDbContext, IHasOutboxCollection
    {
        public BadgeDbContext(BadgeDbClient client, IOptions<MongoOptions> options)
        {
            Database = client.MongoClient.GetDatabase(options.Value.Database);
        }

        public IMongoDatabase Database { get; }

        public IMongoCollection<UserEntity> UserCollection => Database.GetCollection<UserEntity>("Users");

        public IMongoCollection<BadgeEntity> BadgeCollection => Database.GetCollection<BadgeEntity>("Badges");

        public IMongoCollection<UserActivity> ActivityCollection => Database.GetCollection<UserActivity>("UserActivities");

        public IMongoCollection<UserReward> UserRewardCollection => Database.GetCollection<UserReward>("UserRewards");

        public IMongoCollection<YearlyUserStatistic> YearlyUserStatisticCollection => Database.GetCollection<YearlyUserStatistic>("YearlyUserStatistics");

        public IMongoCollection<CommunityYearlyUserStatistic> CommunityYearlyUserStatisticCollection => Database.GetCollection<CommunityYearlyUserStatistic>("CommunityYearlyUserStatistics");

        public IMongoCollection<TopBadgeQualifiedUser> TopBadgeQualifiedUserCollection => Database.GetCollection<TopBadgeQualifiedUser>("TopBadgeQualifiedUsers");

        public IMongoCollection<Community> CommunityCollection => Database.GetCollection<Community>("Communities");

        public IMongoCollection<PostStatistic> PostStatisticCollection =>
            Database.GetCollection<PostStatistic>("PostStatistics");

        public IMongoCollection<BadgeWithCriteria<T>> GetBadgeCriteriaCollection<T>()
            where T : BaseBadgeCriteria
        {
            return Database.GetCollection<BadgeWithCriteria<T>>("Badges");
        }
    }
}
