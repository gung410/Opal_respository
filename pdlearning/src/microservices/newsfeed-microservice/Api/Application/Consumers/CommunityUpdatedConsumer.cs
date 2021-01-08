using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Community = Microservice.NewsFeed.Domain.Entities.Community;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("csl.events.community.updated")]
    [OpalConsumer("csl.events.community.archived")]
    [OpalConsumer("csl.events.community.reactive")]
    [OpalConsumer("csl.events.subcommunity.updated")]
    [OpalConsumer("csl.events.subcommunity.archived")]
    [OpalConsumer("csl.events.subcommunity.reactive")]
    public class CommunityUpdatedConsumer : OpalMessageConsumer<CommunityMessage>
    {
        private readonly ILogger<CommunityUpdatedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public CommunityUpdatedConsumer(
            ILogger<CommunityUpdatedConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CommunityMessage message)
        {
            var filterByCommunityId = Community.FilterByIdExpr(message.Id);
            var existingCommunity = await _dbContext
                .SyncedCommunityCollection
                .AsQueryable()
                .Where(filterByCommunityId)
                .FirstOrDefaultAsync();

            if (existingCommunity == null)
            {
                _logger.LogError("Update community can't found community with {communityId}", message.Id);
                return;
            }

            await UpdateNewsFeedOnCommunityInfoChanged(message, existingCommunity);

            existingCommunity.Update(
                message.Name,
                message.CommunityThumbnail,
                message.Description,
                message.Url,
                message.Visibility,
                message.Status);

            await _dbContext
                .SyncedCommunityCollection
                .ReplaceOneAsync(filterByCommunityId, existingCommunity);
        }

        /// <summary>
        /// Update news feed on community name or community description has changed.
        /// </summary>
        /// <param name="message">Message listen from CSL module.</param>
        /// <param name="existingCommunity">Community entity.</param>
        /// <returns>No results are returned.</returns>
        private async Task UpdateNewsFeedOnCommunityInfoChanged(
            CommunityMessage message,
            Community existingCommunity)
        {
            if (existingCommunity.IsDifferentCommunityName(message.Name)
                || !existingCommunity.IsDifferentCommunityDescription(message.Description))
            {
                await UpdateNewsFeedOnCommunityPostInfoChanged(message);

                await UpdateNewsFeedOnCommunityPostForwardedInfoChanged(message);
            }
        }

        /// <summary>
        /// Community information updates in community posts.
        /// </summary>
        /// <param name="message">Message listen from the CSL module.</param>
        /// <returns>No results are returned.</returns>
        private async Task UpdateNewsFeedOnCommunityPostInfoChanged(CommunityMessage message)
        {
            var filter = Builders<Domain.Entities.NewsFeed>
                .Filter
                .Eq(nameof(CommunityPostFeed.CommunityId), message.Id);

            var update = Builders<Domain.Entities.NewsFeed>.Update
                .Set(nameof(CommunityPostFeed.CommunityName), message.Name)
                .Set(nameof(CommunityPostFeed.Description), message.Description);

            await _dbContext.NewsFeedCollection.UpdateManyAsync(filter, update);
        }

        /// <summary>
        /// Community information updates in forwarded posts.
        /// </summary>
        /// <param name="message">Message listen from the CSL module.</param>
        /// <returns>No results are returned.</returns>
        private async Task UpdateNewsFeedOnCommunityPostForwardedInfoChanged(CommunityMessage message)
        {
            // Filter community in forwarded posts.
            var filterEmbeddedDocument = Builders<Domain.Entities.NewsFeed>
                .Filter
                .Eq($"{nameof(CommunityPostFeed.PostForward)}.{nameof(CommunityPostFeed.CommunityId)}", message.Id);

            // Update community name and description in forwarded posts.
            var updateEmbeddedDocument = Builders<Domain.Entities.NewsFeed>.Update
                .Set($"{nameof(CommunityPostFeed.PostForward)}.{nameof(CommunityPostFeed.CommunityName)}", message.Name)
                .Set($"{nameof(CommunityPostFeed.PostForward)}.{nameof(CommunityPostFeed.Description)}", message.Description);

            await _dbContext.NewsFeedCollection.UpdateManyAsync(filterEmbeddedDocument, updateEmbeddedDocument);
        }
    }
}
