using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.RequestDtos;
using Conexus.Opal.AccessControl.Services;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Uploader.Application.Commands;
using Microservice.Uploader.Common.Helpers;
using Newtonsoft.Json.Linq;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.created.employee")]
    public class UserCreatedConsumer : ScopedOpalMessageConsumer<UserCreatedMessage>
    {
        public async Task InternalHandleAsync(UserCreatedMessage message, IRepository<UserEntity> userRepo, IThunderCqrs thunderCqrs)
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

            var totalSpace = message.UserData.JsonDynamicAttributes[JsonDynamicAttribute.PersonalStorageSize]?.Value<int>()
                                 ?? 10;
            var isStorageUnlimited = message.UserData.JsonDynamicAttributes[JsonDynamicAttribute.IsStorageUnlimited]?.Value<bool>()
                                ?? false;

            var createPersonalSpaceCommand = new SavePersonalSpaceCommand
            {
                UserId = message.UserData.Identity.ExtId,
                TotalSpace = PersonalSpaceHelper.ConvertGigabyteToBytes(totalSpace),
                IsStorageUnlimited = isStorageUnlimited,
                IsCreation = true
            };

            await thunderCqrs.SendCommand(createPersonalSpaceCommand);
        }
    }
}
