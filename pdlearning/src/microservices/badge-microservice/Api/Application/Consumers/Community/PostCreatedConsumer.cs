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
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.post.created")]
    public class PostCreatedConsumer : OpalMessageConsumer<PostCreatedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public PostCreatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(PostCreatedMessage message)
        {
            var postExist = await _dbContext
                .PostStatisticCollection
                .AnyAsync(x => x.PostId == message.Id.ToString());

            if (postExist)
            {
                return;
            }

            await _dbContext.PostStatisticCollection.InsertOneAsync(new PostStatistic
            {
                PostId = message.Id.ToString(),
                CommunityId =
                    message.SourceType == SourceType.Community ? new Guid(message.Source.Id.ToString()) : null,
                CreatedBy = message.CreatedBy,
                HasLink = message.HasLink(),
                NumOfMultimedia = message.Id,
                ModifiedDate = Clock.Now
            });

            var activityType = message.SourceType == SourceType.Community
                ? ActivityType.PostCommunity
                : ActivityType.PostUserWall;
            CommunityInfo communityInfo = new();
            if (message.SourceType == SourceType.Community)
            {
                communityInfo.CommunityId = new Guid(message.Source.Id.ToString());
                communityInfo.OwnerCommunityId = message.Source.CreatedBy;
                communityInfo.HasLink = message.HasLink();
                communityInfo.NumOfMultimedia = message.NumOfMultimedia();
            }

            var userActivity = new UserActivity(message.CreatedBy, message.CreatedAt, message.Id, activityType).WithCommunityInfo(communityInfo);
            await _dbContext.ActivityCollection.InsertOneAsync(userActivity);

            if (message.HasContentForward && message.ContentForwardType == SourceType.Post && message.SourceType == SourceType.Community)
            {
                communityInfo.PostId = message.ContentForward.Id;
                communityInfo.OwnerPostId = message.ContentForward.CreatedBy;

                var newForward = new UserActivity(message.CreatedBy, message.CreatedAt, message.Id.ToString(), ActivityType.Forward)
                    .WithCommunityInfo(communityInfo);
                await _dbContext.ActivityCollection.InsertOneAsync(newForward);
            }
        }
    }
}
