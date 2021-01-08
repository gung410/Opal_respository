using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.community.deleted")]
    [OpalConsumer("csl.events.subcommunity.deleted")]
    public class CommunityDeletedConsumer : OpalMessageConsumer<CommunityChangedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public CommunityDeletedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CommunityChangedMessage message)
        {
            var existedCommunity = await _dbContext
                .CommunityCollection
                .FirstOrDefaultAsync(p => p.Id == message.Id);

            if (existedCommunity == null)
            {
                return;
            }

            await _dbContext.CommunityCollection.DeleteOneAsync(p => p.Id == message.Id);
        }
    }
}
