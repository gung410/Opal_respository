using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Application.Enums;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.comment.created")]
    public class CommentCreatedConsumer : OpalMessageConsumer<CommentCreatedMessage>
    {
        private readonly BadgeDbContext _dbContext;
        private readonly ILogger<CommentCreatedConsumer> _logger;

        public CommentCreatedConsumer(BadgeDbContext dbContext, ILogger<CommentCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async Task InternalHandleAsync(CommentCreatedMessage message)
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

            if (commentExisted || existedPost == null)
            {
                _logger.LogWarning("[CommentCreatedConsumer] Comment post with {CommentId} was existed or post not existed.", message.Id);
                return;
            }

            existedPost.NumOfResponses++;
            await _dbContext.PostStatisticCollection.ReplaceOneAsync(
                p => p.PostId == message.Id.ToString(),
                existedPost);

            var communityInfo = new CommunityInfo
            {
                CommunityId = new Guid(message.Thread.Source.Id.ToString()),
                PostId = message.Thread.Id,
                OwnerPostId = message.Thread.CreatedBy,
                OwnerCommunityId = message.Thread.Source.CreatedBy
            };

            var type = Equals(message.Thread.CreatedBy, message.CreatedBy)
                ? ActivityType.CommentSelfPost
                : ActivityType.CommentOthersPost;

            var newComment = new UserActivity(message.CreatedBy, message.CreatedAt, message.Id, type)
                .WithCommunityInfo(communityInfo);

            await _dbContext.ActivityCollection.InsertOneAsync(newComment);
        }
    }
}
