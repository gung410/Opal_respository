using System;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.user_moved.employee")]
    public class UserDepartmentChangeConsumer : ScopedOpalMessageConsumer<UserDepartmentChangeMessage>
    {
        private readonly ILogger<UserDepartmentChangeConsumer> _logger;

        public UserDepartmentChangeConsumer(ILogger<UserDepartmentChangeConsumer> logger)
        {
            _logger = logger;
        }

        public async Task InternalHandleAsync(UserDepartmentChangeMessage message, IRepository<LearnerUser> userRepo)
        {
            if (message?.DepartmentId == null || message?.UserId == null)
            {
                _logger.LogError($"UserDepartmentChangeConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            var user = await userRepo.FirstOrDefaultAsync(u => u.OriginalUserId == message.UserId);
            if (user != null)
            {
                if (user.DepartmentId != message.DepartmentId)
                {
                    user.DepartmentId = message.DepartmentId;
                    await userRepo.UpdateAsync(user);
                }
            }
        }
    }
}
