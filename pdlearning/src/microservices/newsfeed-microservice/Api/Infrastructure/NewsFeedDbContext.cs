using Microservice.NewsFeed.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Microservice.NewsFeed.Infrastructure
{
    public class NewsFeedDbContext
    {
        public NewsFeedDbContext(NewsFeedDbClient client, IOptions<MongoOptions> options)
        {
            Database = client.MongoClient.GetDatabase(options.Value.Database);
        }

        public IMongoDatabase Database { get; }

        public IMongoCollection<Domain.Entities.NewsFeed> NewsFeedCollection => Database.GetCollection<Domain.Entities.
            NewsFeed>(nameof(Domain.Entities.NewsFeed));

        public IMongoCollection<LearnerCourseSubscription> LearnerCourseSubscriptionCollection =>
            Database.GetCollection<LearnerCourseSubscription>(
                nameof(LearnerCourseSubscription));

        public IMongoCollection<Course> SyncedCourseCollection =>
            Database.GetCollection<Course>(
                nameof(Course));

        public IMongoCollection<Community> SyncedCommunityCollection =>
            Database.GetCollection<Community>(
                nameof(Community));

        public IMongoCollection<User> SyncedUserCollection =>
            Database.GetCollection<User>(
                nameof(User));

        public IMongoCollection<Post> SyncedPostCollection =>
            Database.GetCollection<Post>(
                nameof(Post));
    }
}
