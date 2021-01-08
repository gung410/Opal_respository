using System.Collections.Generic;
using Microservice.Badge.Domain.Entities;
using MongoDB.Driver;

namespace Microservice.Badge.Migrations
{
    public class M_1_0_1_AddIndexesMigration : MongoDBMigrations.IMigration
    {
        public MongoDBMigrations.Version Version => new(1, 0, 1);

        public string Name => "Add indexes";

        public void Up(IMongoDatabase database)
        {
            CreateIndexForUserEntity(database);
        }

        public void Down(IMongoDatabase database)
        {
            // throw new NotImplementedException();
        }

        private void CreateIndexForUserEntity(IMongoDatabase database)
        {
            var userCollection = database.GetCollection<UserEntity>("Users");

            var indexBuilder = Builders<UserEntity>.IndexKeys;

            var fullTextSearchIndex = this.CreateIndexForUserFullTextSearch(indexBuilder);
            userCollection.Indexes.CreateMany(new[]
            {
                fullTextSearchIndex
            });
        }

        private CreateIndexModel<UserEntity> CreateIndexForUserFullTextSearch(IndexKeysDefinitionBuilder<UserEntity> indexBuilder)
        {
            var key = indexBuilder.Combine(new List<IndexKeysDefinition<UserEntity>>
            {
                indexBuilder.Text(p => p.Email),
                indexBuilder.Text(p => p.FirstName),
                indexBuilder.Text(p => p.LastName)
            });

            var options = new CreateIndexOptions
            {
                Name = "UserFullTextSearch"
            };
            var indexModel = new CreateIndexModel<UserEntity>(key, options);

            return indexModel;
        }
    }
}
