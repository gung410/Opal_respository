using System.Collections.Generic;
using Microservice.Badge.Domain.Entities;
using MongoDB.Driver;

namespace Microservice.Badge.Migrations
{
    public class M_1_0_2_AddQualifiedUserIndexesMigration : MongoDBMigrations.IMigration
    {
        public MongoDBMigrations.Version Version => new(1, 0, 2);

        public string Name => "Add qualified user indexes";

        public void Up(IMongoDatabase database)
        {
            CreateIndexForQualifiedUserBadge(database);
        }

        public void Down(IMongoDatabase database)
        {
            // throw new NotImplementedException();
        }

        private void CreateIndexForQualifiedUserBadge(IMongoDatabase database)
        {
            var qualifiedUserBadgeCollection = database.GetCollection<TopBadgeQualifiedUser>("TopBadgeQualifiedUsers");

            var indexBuilder = Builders<TopBadgeQualifiedUser>.IndexKeys;

            var fullTextSearchIndex = this.CreateIndexForUserFullTextSearch(indexBuilder);
            var coupleIdIndex = this.CreateIndexForCombineId(indexBuilder);

            qualifiedUserBadgeCollection.Indexes.CreateMany(new[]
            {
                fullTextSearchIndex,
                coupleIdIndex
            });
        }

        private CreateIndexModel<TopBadgeQualifiedUser> CreateIndexForUserFullTextSearch(IndexKeysDefinitionBuilder<TopBadgeQualifiedUser> indexBuilder)
        {
            var key = indexBuilder.Combine(new List<IndexKeysDefinition<TopBadgeQualifiedUser>>
            {
                indexBuilder.Text(p => p.Email),
                indexBuilder.Text(p => p.FirstName),
                indexBuilder.Text(p => p.LastName)
            });
            var options = new CreateIndexOptions
            {
                Name = "UserFullTextSearch"
            };
            var indexModel = new CreateIndexModel<TopBadgeQualifiedUser>(key, options);

            return indexModel;
        }

        private CreateIndexModel<TopBadgeQualifiedUser> CreateIndexForCombineId(IndexKeysDefinitionBuilder<TopBadgeQualifiedUser> indexBuilder)
        {
            var key = indexBuilder.Combine(new List<IndexKeysDefinition<TopBadgeQualifiedUser>>
            {
                indexBuilder.Ascending(p => p.UserId),
                indexBuilder.Ascending(p => p.BadgeId)
            });

            var options = new CreateIndexOptions
            {
                Name = "UserIdBadgeId"
            };
            var indexModel = new CreateIndexModel<TopBadgeQualifiedUser>(key, options);

            return indexModel;
        }
    }
}
