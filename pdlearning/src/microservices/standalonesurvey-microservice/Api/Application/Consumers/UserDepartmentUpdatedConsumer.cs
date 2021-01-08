using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.user_moved.employee")]
    public class UserDepartmentChangeConsumer : ScopedOpalMessageConsumer<UserDepartmentChangeMessage>
    {
        public async Task InternalHandleAsync(UserDepartmentChangeMessage message, IRepository<SyncedUser> userRepo)
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
