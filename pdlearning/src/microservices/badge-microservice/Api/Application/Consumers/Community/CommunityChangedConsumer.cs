using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.community.updated")]
    [OpalConsumer("csl.events.subcommunity.updated")]
    public class CommunityChangedConsumer : OpalMessageConsumer<CommunityChangedMessage>
    {
        private readonly BadgeDbContext _dbContext;
        private readonly ILogger<CommunityChangedConsumer> _logger;

        public CommunityChangedConsumer(
            BadgeDbContext dbContext,
            ILogger<CommunityChangedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async Task InternalHandleAsync(CommunityChangedMessage message)
        {
            var existedCommunity = await _dbContext
                .CommunityCollection
                .FirstOrDefaultAsync(p => p.Id == message.Id);

            if (existedCommunity == null)
            {
                _logger.LogError("Update community can't found community with {communityId}", message.Id);
                return;
            }

            existedCommunity.UpdateName(message.Name);

            await _dbContext.CommunityCollection.ReplaceOneAsync(p => p.Id == message.Id, existedCommunity);
        }
    }
}
