using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("csl.events.post.deleted")]
    public class UserPostDeletedConsumer : OpalMessageConsumer<UserPostMessage>
    {
        private readonly ILogger<UserPostDeletedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public UserPostDeletedConsumer(
            ILogger<UserPostDeletedConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(UserPostMessage message)
        {
            var existedPost = await _dbContext
                .SyncedPostCollection
                .AsQueryable()
                .Where(p => p.PostId == (int)message.Source.Id)
                .AnyAsync();

            if (existedPost)
            {
                _logger.LogError("UserPostDeleted: The post not found.");
                return;
            }

            await _dbContext
                .SyncedPostCollection
                .DeleteOneAsync(p => p.PostId == (int)message.Source.Id);
        }
    }
}
