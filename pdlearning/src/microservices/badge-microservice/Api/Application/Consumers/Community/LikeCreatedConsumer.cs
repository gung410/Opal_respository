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
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.like.created")]
    public class LikeCreatedConsumer : OpalMessageConsumer<LikeCreatedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public LikeCreatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(LikeCreatedMessage message)
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

            if (likePostExisted || existedPost == null)
            {
                return;
            }

            existedPost.NumOfResponses++;
            await _dbContext.PostStatisticCollection.ReplaceOneAsync(
                p => p.PostId == message.Id.ToString(),
                existedPost);

            var communityInfo = new CommunityInfo
            {
                PostId = Convert.ToInt32(message.Source.Id.ToString()),
                OwnerPostId = message.Source.CreatedBy
            };
            var newLikePost = new UserActivity(message.CreatedBy, message.CreatedAt, message.Id, ActivityType.LikePost)
                .WithCommunityInfo(communityInfo);
            await _dbContext.ActivityCollection.InsertOneAsync(newLikePost);
        }
    }
}
