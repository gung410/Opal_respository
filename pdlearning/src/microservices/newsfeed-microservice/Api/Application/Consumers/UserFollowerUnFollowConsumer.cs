using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using User = Microservice.NewsFeed.Domain.Entities.User;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("csl.events.follow.unfollow")]
    public class UserFollowerUnFollowConsumer : OpalMessageConsumer<UserFollowMessage>
    {
        private readonly ILogger<UserFollowerUnFollowConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public UserFollowerUnFollowConsumer(
            ILogger<UserFollowerUnFollowConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(UserFollowMessage message)
        {
            if (!message.IsUserFollowUser())
            {
                return;
            }

            var filterByExtId = User.FilterByExtIdExpr(message.Target.Guid);
            var user = await _dbContext
                .SyncedUserCollection
                .AsQueryable()
                .Where(filterByExtId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogError("UserFollowerUnFollow: User not found.");
                return;
            }

            user.RemoveFollower(message.User.Guid);

            await _dbContext
                .SyncedUserCollection
                .ReplaceOneAsync(filterByExtId, user);
        }
    }
}
