using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.post.updated")]
    public class PostUpdatedConsumer : OpalMessageConsumer<PostCreatedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public PostUpdatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(PostCreatedMessage message)
        {
            var existedPost = await _dbContext
                .PostStatisticCollection
                .FirstOrDefaultAsync(x => x.PostId == message.Id.ToString());
            var existedActivity = await _dbContext
                .ActivityCollection
                .FirstOrDefaultAsync(x =>
                    x.SourceId == message.Id.ToString() &&
                    (x.Type == ActivityType.PostCommunity || x.Type == ActivityType.PostUserWall));
            if (existedPost == null || existedActivity == null)
            {
                return;
            }

            existedPost.ModifiedDate = Clock.Now;
            existedPost.HasLink = message.HasLink();
            existedPost.NumOfMultimedia = message.NumOfMultimedia();
            await _dbContext.PostStatisticCollection.ReplaceOneAsync(
                p => p.PostId == message.Id.ToString(),
                existedPost);

            if (existedActivity.Type == ActivityType.PostCommunity)
            {
                existedActivity.CommunityInfo.HasLink = message.HasLink();
                existedActivity.CommunityInfo.NumOfMultimedia = message.NumOfMultimedia();
                await _dbContext.ActivityCollection.ReplaceOneAsync(x => x.Id == existedActivity.Id, existedActivity);
            }
        }
    }
}
