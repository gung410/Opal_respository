using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.RequestDtos;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.Extensions.Logging;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.updated.employee")]
    public class UserUpdatedConsumer : ScopedOpalMessageConsumer<UserCreatedMessage>
    {
        private readonly ILogger<UserUpdatedConsumer> _logger;
        private readonly IWriteLearnerUserLogic _writeLearnerUserLogic;

        public UserUpdatedConsumer(
            ILogger<UserUpdatedConsumer> logger,
            IWriteLearnerUserLogic writeLearnerUserLogic)
        {
            _logger = logger;
            _writeLearnerUserLogic = writeLearnerUserLogic;
        }

        public async Task InternalHandleAsync(UserCreatedMessage message)
        {
            if (message.UserData?.Identity?.ExtId == null)
            {
                _logger.LogError($"UserUpdatedConsumer invalid data: {JsonSerializer.Serialize(message)}");
                return;
            }

            await _writeLearnerUserLogic.SaveLearnerUser(
                new SaveLearnerUserRequestDto
                {
                    DepartmentId = message.DepartmentId,
                    EmailAddress = message.UserData.EmailAddress,
                    FirstName = message.UserData.FirstName,
                    LastName = message.UserData.LastName,
                    Identity = new SaveLearnerUserRequestDtoIdentity
                    {
                        ExtId = message.UserData.Identity.ExtId,
                        Id = message.UserData.Identity.Id
                    },
                    EntityStatus = new SaveLearnerUserRequestDtoEntityStatus
                    {
                        ExternallyMastered = message.UserData.EntityStatus.ExternallyMastered,
                        Status = message.UserData.EntityStatus.StatusId
                    },
                    PrimaryApprovingOfficerId = message.UserData.Groups?
                            .FirstOrDefault(p => p.Type == LearnerUserGroupType.PrimaryApprovalGroup)?.UserIdentity.ExtId
                            ?? Guid.Empty,
                    AlternativeApprovingOfficerId = message.UserData.Groups?
                            .Where(p => p.Type == LearnerUserGroupType.AlternativeApprovalGroup)
                            .Select(x => x.UserIdentity.ExtId)
                            .FirstOrDefault(),
                });
        }
    }
}
