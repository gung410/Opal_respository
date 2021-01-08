using System.Collections.Generic;
using Microservice.NewsFeed.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Version = MongoDBMigrations.Version;

namespace Microservice.NewsFeed.Migrations
{
    public class M_1_0_1_AddIndexesMigration : MongoDBMigrations.IMigration
    {
        public MongoDBMigrations.Version Version => new Version(1, 0, 1);

        public string Name => "Add indexes";

        public void Up(IMongoDatabase database)
        {
            CreateIndexForNewsFeed(database);
            CreateIndexForSyncedCommunity(database);
            CreateIndexForSyncedCourse(database);
            CreateIndexForSyncedUser(database);
            CreateIndexForSyncedPost(database);
        }

        public void Down(IMongoDatabase database)
        {
            // throw new NotImplementedException();
        }

        private void CreateIndexForSyncedPost(IMongoDatabase database)
        {
            var postsCollection = database.GetCollection<BsonDocument>(nameof(Post));

            var indexBuilder = Builders<BsonDocument>.IndexKeys;

            var indexForPostId = this.CreateIndexForPostId(indexBuilder);

            postsCollection.Indexes.CreateMany(new[]
            {
                indexForPostId
            });
        }

        private CreateIndexModel<BsonDocument> CreateIndexForPostId(IndexKeysDefinitionBuilder<BsonDocument> indexBuilder)
        {
            var key = indexBuilder.Ascending(nameof(Post.PostId));

            var options = new CreateIndexOptions
            {
                Name = "PostId_1"
            };
            var indexModel = new CreateIndexModel<BsonDocument>(key, options);

            return indexModel;
        }

        private void CreateIndexForSyncedUser(IMongoDatabase database)
        {
            var usersCollection = database.GetCollection<BsonDocument>(nameof(User));

            var indexBuilder = Builders<BsonDocument>.IndexKeys;

            var indexForExtId = this.CreateIndexForExtId(indexBuilder);

            usersCollection.Indexes.CreateMany(new[]
            {
                indexForExtId
            });
        }

        private CreateIndexModel<BsonDocument> CreateIndexForExtId(IndexKeysDefinitionBuilder<BsonDocument> indexBuilder)
        {
            var key = indexBuilder.Ascending(nameof(User.ExtId));

            var options = new CreateIndexOptions
            {
                Name = "ExtId_1"
            };
            var indexModel = new CreateIndexModel<BsonDocument>(key, options);

            return indexModel;
        }

        private void CreateIndexForSyncedCourse(IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<BsonDocument>(nameof(Course));

            var indexBuilder = Builders<BsonDocument>.IndexKeys;

            var indexForCourseId = this.CreateIndexForCourseId(indexBuilder);

            coursesCollection.Indexes.CreateMany(new[]
            {
                indexForCourseId
            });
        }

        private CreateIndexModel<BsonDocument> CreateIndexForCourseId(IndexKeysDefinitionBuilder<BsonDocument> indexBuilder)
        {
            var key = indexBuilder.Ascending(nameof(Course.CourseId));

            var options = new CreateIndexOptions
            {
                Name = "CourseId_1"
            };
            var indexModel = new CreateIndexModel<BsonDocument>(key, options);

            return indexModel;
        }

        private void CreateIndexForSyncedCommunity(IMongoDatabase database)
        {
            var communitiesCollection = database.GetCollection<BsonDocument>(nameof(Community));

            var indexBuilder = Builders<BsonDocument>.IndexKeys;

            var indexForCommunityId = this.CreateIndexForCommunityId(indexBuilder);

            communitiesCollection.Indexes.CreateMany(new[]
            {
                indexForCommunityId
            });
        }

        private CreateIndexModel<BsonDocument> CreateIndexForCommunityId(IndexKeysDefinitionBuilder<BsonDocument> indexBuilder)
        {
            var key = indexBuilder.Ascending(nameof(Community.CommunityId));

            var options = new CreateIndexOptions
            {
                Name = "CommunityId_1"
            };
            var indexModel = new CreateIndexModel<BsonDocument>(key, options);

            return indexModel;
        }

        private void CreateIndexForNewsFeed(IMongoDatabase database)
        {
            var newsFeedCollection = database.GetCollection<BsonDocument>(nameof(NewsFeed));

            var indexBuilder = Builders<BsonDocument>.IndexKeys;

            var queryNewsFeedIndex = this.CreateIndexForQueryNewsFeed(indexBuilder);
            var indexForCourseIdNewsFeed = this.CreateIndexForCourseIdNewsFeed(indexBuilder);

            newsFeedCollection.Indexes.CreateMany(new[]
            {
                queryNewsFeedIndex,
                indexForCourseIdNewsFeed
            });
        }

        private CreateIndexModel<BsonDocument> CreateIndexForCourseIdNewsFeed(IndexKeysDefinitionBuilder<BsonDocument> indexBuilder)
        {
            var key = indexBuilder.Ascending(nameof(PdpmSuggestCourseFeed.CourseId));

            var options = new CreateIndexOptions
            {
                Name = "PdpmSuggestCourseFeed.CourseId_1"
            };
            var indexModel = new CreateIndexModel<BsonDocument>(key, options);

            return indexModel;
        }

        private CreateIndexModel<BsonDocument> CreateIndexForQueryNewsFeed(
            IndexKeysDefinitionBuilder<BsonDocument> indexBuilder)
        {
            var keys = indexBuilder.Combine(new List<IndexKeysDefinition<BsonDocument>>()
            {
                indexBuilder.Ascending(nameof(Domain.Entities.NewsFeed.UserId)),
                indexBuilder.Descending(nameof(Domain.Entities.NewsFeed.CreatedDate))
            });

            var options = new CreateIndexOptions
            {
                Name = "IndexForQueryNewsFeed"
            };
            var indexModel = new CreateIndexModel<BsonDocument>(keys, options);

            return indexModel;
        }
    }
}
