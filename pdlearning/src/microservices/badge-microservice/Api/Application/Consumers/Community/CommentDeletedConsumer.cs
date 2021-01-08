using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Application.Enums;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.comment.deleted")]
    public class CommentDeletedConsumer : OpalMessageConsumer<CommentDeletedMessage>
    {
        private readonly BadgeDbContext _dbContext;
        private readonly ILogger<CommentDeletedConsumer> _logger;

        public CommentDeletedConsumer(BadgeDbContext dbContext, ILogger<CommentDeletedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async Task InternalHandleAsync(CommentDeletedMessage message)
        {
            if (message.ThreadType != ThreadType.Post)
            {
                return;
            }

            var commentExisted = await _dbContext
                .ActivityCollection
                .AnyAsync(x => x.SourceId == message.Id.ToString()
                               && (x.Type == ActivityType.CommentOthersPost || x.Type == ActivityType.CommentSelfPost));
            var existedPost = await _dbContext
                .PostStatisticCollection
                .FirstOrDefaultAsync(x => x.PostId == message.Id.ToString());

            if (!commentExisted || existedPost == null)
            {
                _logger.LogWarning("[CommentDeletedConsumer] Comment post with {CommentId} was not existed or post not existed.", message.Id);
                return;
            }

            existedPost.NumOfResponses--;
            await _dbContext.PostStatisticCollection.ReplaceOneAsync(
                p => p.PostId == message.Id.ToString(),
                existedPost);

            await _dbContext.ActivityCollection.DeleteOneAsync(x => x.SourceId == message.Id.ToString()
                                                                    && (x.Type == ActivityType.CommentOthersPost || x.Type == ActivityType.CommentSelfPost));
        }
    }
}
