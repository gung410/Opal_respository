using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.entitystatus_changed.employee")]
    public class UserEntityStatusChangedConsumer : ScopedOpalMessageConsumer<UserEntityStatusChangeMessage>
    {
        public async Task InternalHandleAsync(
            UserEntityStatusChangeMessage message,
            SaveUserLogic saveUserLogic)
        {
            await saveUserLogic.SaveCourseUserEntityStatus(
                new SaveCourseUserRequestDto
                {
                    EntityStatus = new SaveCourseUserRequestDtoEntityStatus { Status = message.UserData.ToEntityStatusId },
                    Identity = new SaveCourseUserRequestDtoIdentity { ExtId = message.UserCxId, Id = message.UserId },
                });
        }
    }
}
