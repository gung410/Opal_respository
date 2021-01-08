using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.forum.created")]
    public class ForumCreatedConsumer : OpalMessageConsumer<ForumCreatedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public ForumCreatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(ForumCreatedMessage message)
        {
            var forumExisted = await _dbContext
                .ActivityCollection
                .AnyAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.CreateForum);

            if (forumExisted)
            {
                return;
            }

            var communityInfo = new CommunityInfo
            {
                CommunityId = new Guid(message.Source.Id.ToString()),
                OwnerCommunityId = message.Source.CreatedBy
            };

            var newForum = new UserActivity(message.CreatedBy, Clock.Now, message.Id.ToString(), ActivityType.CreateForum)
                .WithCommunityInfo(communityInfo);

            await _dbContext.ActivityCollection.InsertOneAsync(newForum);
        }
    }
}
