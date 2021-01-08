using System;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.RequestDtos;
using Conexus.Opal.AccessControl.Services;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Content.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.created.employee")]
    public class UserCreatedConsumer : ScopedOpalMessageConsumer<UserCreatedMessage>
    {
        public async Task InternalHandleAsync(UserCreatedMessage message, IRepository<UserEntity> userRepo)
        {
            await UserSaver.SaveUser(
                new SaveUserRequestDto
                {
                    DepartmentId = message.DepartmentId,
                    EmailAddress = message.UserData.EmailAddress,
                    FirstName = message.UserData.FirstName,
                    LastName = message.UserData.LastName,
                    Identity = new SaveUserRequestDtoIdentity
                    {
                        ExtId = message.UserData.Identity.ExtId,
                        Id = message.UserData.Identity.Id
                    }
                },
                userRepo);
        }
    }
}
