using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Uploader.Application.Commands;
using Microservice.Uploader.Common.Helpers;
using Newtonsoft.Json.Linq;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.updated.employee-syncdata")]
    public class UserPersonalSpaceUpdatedConsumer : ScopedOpalMessageConsumer<UserPersonalSpaceChangeMessage>
    {
        public async Task InternalHandleAsync(UserPersonalSpaceChangeMessage message, IThunderCqrs thunderCqrs)
        {
            var totalSpace = message.UserData.JsonDynamicAttributes[JsonDynamicAttribute.PersonalStorageSize]?.Value<int>()
                                ?? 10;
            var isStorageUnlimited = message.UserData.JsonDynamicAttributes[JsonDynamicAttribute.IsStorageUnlimited]?.Value<bool>()
                                ?? false;

            var createPersonalSpaceCommand = new SavePersonalSpaceCommand
            {
                UserId = message.UserData.Identity.ExtId,
                TotalSpace = PersonalSpaceHelper.ConvertGigabyteToBytes(totalSpace),
                IsStorageUnlimited = isStorageUnlimited,
                IsCreation = false
            };

            await thunderCqrs.SendCommand(createPersonalSpaceCommand);
        }
    }
}
