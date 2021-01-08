using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.updated.employee")]
    [OpalConsumer("cx-organization-api.crud.created.employee")]
    public class UserUpsertedConsumer : ScopedOpalMessageConsumer<UserCreatedMessage>
    {
        public async Task InternalHandleAsync(UserCreatedMessage message, IRepository<SyncedUser> userRepo)
        {
            var user = await userRepo.FirstOrDefaultAsync(u => u.OriginalUserId == message.UserData.Identity.Id && u.Id == message.UserData.Identity.ExtId);
            if (user == null)
            {
                user = new SyncedUser
                {
                    Id = message.UserData.Identity.ExtId,
                    OriginalUserId = message.UserData.Identity.Id,
                    DepartmentId = message.DepartmentId,
                    Email = message.UserData.EmailAddress,
                    FirstName = message.UserData.FirstName,
                    LastName = message.UserData.LastName,
                    DepartmentName = message.UserData.DepartmentName
                };
                await userRepo.InsertAsync(user);
            }
            else
            {
                user.DepartmentId = message.DepartmentId;
                user.Email = message.UserData.EmailAddress;
                user.FirstName = message.UserData.FirstName;
                user.LastName = message.UserData.LastName;
                user.DepartmentName = message.UserData.DepartmentName;

                await userRepo.UpdateAsync(user);
            }
        }
    }
}
