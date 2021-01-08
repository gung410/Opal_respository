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
    [OpalConsumer("csl.events.community.created")]
    [OpalConsumer("csl.events.subcommunity.created")]
    public class CommunityCreatedConsumer : OpalMessageConsumer<CommunityMessage>
    {
        private readonly ILogger<CommunityCreatedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public CommunityCreatedConsumer(
            ILogger<CommunityCreatedConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CommunityMessage message)
        {
            var existedCommunity = await _dbContext
                .SyncedCommunityCollection
                .AsQueryable()
                .Where(Community.FilterByIdExpr(message.Id))
                .AnyAsync();

            if (existedCommunity)
            {
                _logger.LogError("Existed community with {communityId}", message.Id);
                return;
            }

            var community = new Community(
                communityId: message.Id,
                message.Description,
                message.Name,
                message.CommunityThumbnail,
                message.Visibility,
                message.Url,
                message.MainCommunityId,
                message.Status,
                message.CreatedBy,
                message.UpdatedBy,
                message.CreatedAt,
                message.CreatedAt);

            community.AddMembership(message.CreatedBy);

            await _dbContext.SyncedCommunityCollection.InsertOneAsync(community);
        }
    }
}
