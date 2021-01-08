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
    [OpalConsumer("csl.events.like.deleted")]
    public class LikeDeletedConsumer : OpalMessageConsumer<LikeDeletedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public LikeDeletedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(LikeDeletedMessage message)
        {
            if (message.SourceType != SourceType.Post)
            {
                return;
            }

            var likePostExisted = await _dbContext
                .ActivityCollection
                .AnyAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.LikePost);
            var existedPost = await _dbContext
                .PostStatisticCollection
                .FirstOrDefaultAsync(x => x.PostId == message.Id.ToString());

            if (!likePostExisted || existedPost == null)
            {
                return;
            }

            existedPost.NumOfResponses--;
            await _dbContext.PostStatisticCollection.ReplaceOneAsync(
                p => p.PostId == message.Id.ToString(),
                existedPost);

            await _dbContext.ActivityCollection.DeleteOneAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.LikePost);
        }
    }
}
