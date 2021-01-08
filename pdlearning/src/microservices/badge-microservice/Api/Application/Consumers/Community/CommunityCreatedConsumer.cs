using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.community.created")]
    [OpalConsumer("csl.events.subcommunity.created")]
    public class CommunityCreatedConsumer : OpalMessageConsumer<CommunityChangedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public CommunityCreatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CommunityChangedMessage message)
        {
            var existedCommunity = await _dbContext
                .CommunityCollection
                .FirstOrDefaultAsync(p => p.Id == message.Id);

            if (existedCommunity != null)
            {
                return;
            }

            var community = new Community(message.Id, message.Name);

            await _dbContext.CommunityCollection.InsertOneAsync(community);
        }
    }
}
