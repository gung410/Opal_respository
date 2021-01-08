using System;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.LnaForm.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.user_moved.employee")]
    public class UserDepartmentChangeConsumer : ScopedOpalMessageConsumer<UserDepartmentChangeMessage>
    {
        public async Task InternalHandleAsync(UserDepartmentChangeMessage message, IRepository<UserEntity> userRepo)
        {
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
