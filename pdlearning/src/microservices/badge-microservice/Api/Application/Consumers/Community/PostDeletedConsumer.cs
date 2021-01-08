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
    [OpalConsumer("csl.events.post.deleted")]
    public class PostDeletedConsumer : OpalMessageConsumer<PostDeletedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public PostDeletedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(PostDeletedMessage message)
        {
            var postExist = await _dbContext
                .PostStatisticCollection
                .AnyAsync(x => x.PostId == message.Id.ToString());

            if (!postExist)
            {
                return;
            }

            await _dbContext.PostStatisticCollection.DeleteOneAsync(p => p.PostId == message.Id.ToString());

            await _dbContext.ActivityCollection.DeleteOneAsync(
                x => x.SourceId == message.Id.ToString() &&
                     (x.Type == ActivityType.PostCommunity || x.Type == ActivityType.PostUserWall));
            if (message.HasContentForward && message.ContentForwardType == SourceType.Post && message.SourceType == SourceType.Community)
            {
                await _dbContext.ActivityCollection.DeleteOneAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.Forward);
            }
        }
    }
}
