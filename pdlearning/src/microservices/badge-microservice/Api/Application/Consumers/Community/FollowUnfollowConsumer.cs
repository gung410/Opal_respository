using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Application.Enums;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.follow.unfollow")]
    public class FollowUnfollowConsumer : OpalMessageConsumer<FollowUnfollowedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public FollowUnfollowConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(FollowUnfollowedMessage message)
        {
            if (message.TargetType != TargetType.Community)
            {
                return;
            }

            var followExisted = await _dbContext
                .ActivityCollection
                .AnyAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.FollowCommunity);

            if (!followExisted)
            {
                return;
            }

            await _dbContext.ActivityCollection.DeleteOneAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.FollowCommunity);
        }
    }
}
