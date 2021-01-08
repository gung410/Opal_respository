using System.Threading.Tasks;
using Conexus.Opal.AccessControl.RequestDtos;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Application.Services;

namespace Microservice.Webinar.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.updated.employee")]
    public class UserUpdatedConsumer : ScopedOpalMessageConsumer<UserCreatedMessage>
    {
        private readonly IWebinarUserService _webinarUserService;

        public UserUpdatedConsumer(IWebinarUserService webinarUserService)
        {
            _webinarUserService = webinarUserService;
        }

        public async Task InternalHandleAsync(UserCreatedMessage message)
        {
            await _webinarUserService.SaveUser(
                new SaveWebinarUserRequestDto
                {
                    AvatarUrl = message.UserData.JsonDynamicAttributes?.AvatarUrl,
                    DepartmentId = message.DepartmentId,
                    EmailAddress = message.UserData.EmailAddress,
                    FirstName = message.UserData.FirstName,
                    LastName = message.UserData.LastName,
                    Identity = new SaveUserRequestDtoIdentity
                    {
                        ExtId = message.UserData.Identity.ExtId,
                        Id = message.UserData.Identity.Id
                    }
                });
        }
    }
}
