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
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.follow.created")]
    public class FollowCreatedConsumer : OpalMessageConsumer<FollowCreatedMessage>
    {
        private readonly BadgeDbContext _dbContext;
        private readonly ILogger<FollowCreatedConsumer> _logger;

        public FollowCreatedConsumer(BadgeDbContext dbContext, ILogger<FollowCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async Task InternalHandleAsync(FollowCreatedMessage message)
        {
            if (message.TargetType != TargetType.Community)
            {
                return;
            }

            var followExisted = await _dbContext
                .ActivityCollection
                .AnyAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.FollowCommunity);

            if (followExisted)
            {
                _logger.LogWarning("[FollowCreatedConsumer] Follow community with {FollowId} was existed.", message.Id);
                return;
            }

            var communityInfo = new CommunityInfo
            {
                CommunityId = new Guid(message.Target.Id.ToString())
            };

            var newFollowCommunity = new UserActivity(message.User.Guid, Clock.Now, message.Id, ActivityType.FollowCommunity)
                .WithCommunityInfo(communityInfo);

            await _dbContext.ActivityCollection.InsertOneAsync(newFollowCommunity);
        }
    }
}
