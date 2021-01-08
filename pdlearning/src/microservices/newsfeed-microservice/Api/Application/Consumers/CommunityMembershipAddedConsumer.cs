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
    [OpalConsumer("csl.events.membership.added")]
    public class CommunityMembershipAddedConsumer : OpalMessageConsumer<CommunityMembershipMessage>
    {
        private readonly ILogger<CommunityMembershipAddedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public CommunityMembershipAddedConsumer(
            ILogger<CommunityMembershipAddedConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CommunityMembershipMessage message)
        {
            var filterByCommunityId = Community.FilterByIdExpr(message.Community.Id);
            var community = await _dbContext
                .SyncedCommunityCollection
                .AsQueryable()
                .Where(filterByCommunityId)
                .FirstOrDefaultAsync();

            if (community == null)
            {
                _logger.LogError("Update community can't found community with {communityId}", message.Community.Id);
                return;
            }

            community.AddMembership(message.User.Guid);

            await _dbContext
                .SyncedCommunityCollection
                .ReplaceOneAsync(filterByCommunityId, community);
        }
    }
}
