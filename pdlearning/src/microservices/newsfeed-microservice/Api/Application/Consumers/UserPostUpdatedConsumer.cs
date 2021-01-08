using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Domain.ValueObject;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using User = Microservice.NewsFeed.Domain.Entities.User;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("csl.events.post.updated")]
    public class UserPostUpdatedConsumer : OpalMessageConsumer<UserPostMessage>
    {
        private readonly ILogger<UserPostUpdatedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public UserPostUpdatedConsumer(ILogger<UserPostUpdatedConsumer> logger, NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(UserPostMessage message)
        {
            var postFilter = Post.FilterByPostId((int)message.Source.Id);

            var post = await _dbContext
                .SyncedPostCollection
                .AsQueryable()
                .Where(postFilter)
                .FirstOrDefaultAsync();

            // Change the privacy of the post from private to public
            if (!post.IsPostPublic()
                && message.IsPostPublic()
                && message.IsUserSelfPosted()
                && message.IsPostFromUserWall())
            {
                var user = await _dbContext
                    .SyncedUserCollection
                    .AsQueryable()
                    .Where(User.FilterByExtIdExpr(message.CreatedBy))
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogError("UserPostUpdated: User not found.");
                    return;
                }

                var userPostFeeds = user.Followers
                    .Select(userId =>
                    {
                        var userPostFeed = new UserPostFeed
                        {
                            CreatedDate = message.CreatedAt,
                            ChangedDate = message.UpdatedAt,
                            PostContent = message.PostContent,
                            PostId = message.PostId,
                            PostType = PostType.Post,
                            PostedBy = message.CreatedBy,
                            Url = message.Source.Url
                        };

                        userPostFeed.AddUserId(userId);

                        return userPostFeed;
                    });

                await _dbContext.NewsFeedCollection.InsertManyAsync(userPostFeeds);
            }

            post.Update(
                message.PostContent,
                message.Source.Url,
                message.SourceType,
                message.Visibility,
                message.UpdatedBy,
                message.UpdatedAt);

            await _dbContext.SyncedPostCollection.ReplaceOneAsync(postFilter, post);
        }
    }
}
