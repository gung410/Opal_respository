using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Community = Microservice.NewsFeed.Domain.Entities.Community;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("csl.events.community.deleted")]
    [OpalConsumer("csl.events.subcommunity.deleted")]
    public class CommunityDeletedConsumer : OpalMessageConsumer<CommunityMessage>
    {
        private readonly ILogger<CommunityDeletedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public CommunityDeletedConsumer(
            ILogger<CommunityDeletedConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CommunityMessage message)
        {
            var filterByCommunityId = Community.FilterByIdExpr(message.Id);
            var existedCommunity = await _dbContext
                .SyncedCommunityCollection
                .AsQueryable()
                .Where(filterByCommunityId)
                .AnyAsync();

            if (!existedCommunity)
            {
                _logger.LogError("CommunityDeleted: Community not found {communityId}", message.Id);
                return;
            }

            await _dbContext
                .SyncedCommunityCollection
                .DeleteOneAsync(filterByCommunityId);
        }
    }
}
