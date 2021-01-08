using System;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("csl.events.follow.created")]
    public class UserFollowConsumer : ScopedOpalMessageConsumer<UserFollowingMessage>
    {
        private readonly ILogger<UserFollowConsumer> _logger;

        public UserFollowConsumer(ILogger<UserFollowConsumer> logger)
        {
            _logger = logger;
        }

        public async Task InternalHandleAsync(
            UserFollowingMessage message,
            IRepository<UserFollowing> userFollowingRepository)
        {
            if (!ValidateMessage(message))
            {
                _logger.LogError($"UserFollowConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            // Check if registrationId existed in myClassRun we must be bypass to avoid spam redeliver message from another system (like current Reporting)
            var existingClassRun = await userFollowingRepository
                .FirstOrDefaultAsync(p =>
                    p.UserId == message.User.Guid
                    && p.FollowingUserId == message.Target.Guid);
            if (existingClassRun != null)
            {
                // Implement Idempotent to avoid duplicate data come when the message can redeliver in RabbitMQ
                return;
            }

            var userFollowing = new UserFollowing
            {
                Id = Guid.NewGuid(),
                UserId = message.User.Guid,
                FollowingUserId = message.Target.Guid,
                CreatedBy = message.User.Guid,
                CreatedDate = Clock.Now
            };

            await userFollowingRepository.InsertAsync(userFollowing);
        }

        private bool ValidateMessage(UserFollowingMessage message)
        {
            return message.User != null && message.Target != null && message.TargetType == "user";
        }
    }
}
