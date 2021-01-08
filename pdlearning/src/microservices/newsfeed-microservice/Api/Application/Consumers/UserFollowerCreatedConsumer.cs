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
    [OpalConsumer("csl.events.follow.created")]
    public class UserFollowerCreatedConsumer : OpalMessageConsumer<UserFollowMessage>
    {
        private readonly ILogger<UserFollowerCreatedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public UserFollowerCreatedConsumer(
            ILogger<UserFollowerCreatedConsumer> logger,
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
                _logger.LogError("UserFollowerCreated: User not found.");
                return;
            }

            user.AddFollower(message.User.Guid);

            await _dbContext
                .SyncedUserCollection
                .ReplaceOneAsync(filterByExtId, user);
        }
    }
}
