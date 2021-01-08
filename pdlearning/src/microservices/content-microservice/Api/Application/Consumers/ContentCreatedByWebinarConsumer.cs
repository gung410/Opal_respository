using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Content.Application.Commands;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Versioning.Application.Dtos;
using Microservice.Content.Versioning.Entities;
using Microservice.Content.Versioning.Services;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Consumers
{
    [OpalConsumer("microservice.events.content.create")]
    public class ContentCreatedByWebinarConsumer : ScopedOpalMessageConsumer<WebinarInfoMessage>
    {
        public async Task InternalHandleAsync(
            WebinarInfoMessage message,
            IRepository<UserEntity> userRepository,
            IRepository<AccessRight> accessRightRepository,
            IThunderCqrs thunderCqrs,
            IVersionTrackingApplicationService versionTrackingApplicationService)
        {
            var userDepartment = await userRepository.FirstOrDefaultAsync(d => d.Id == message.OwnerId);

            var saveCommand = new SaveDigitalContentCommand
            {
                CreationRequest = new CreateDigitalContentRequest
                {
                    Id = message.Id,
                    Title = message.Title ?? "Draft",
                    Status = message.Status,
                    Type = message.Type,
                    FileExtension = message.FileExtension,
                    FileLocation = message.FileLocation,
                    FileName = message.FileName,
                    FileType = message.FileType,
                    FileSize = message.FileSize,
                    AttributionElements = new List<CreateAttributionElementRequest>()
                },

                UserId = message.OwnerId,
                DepartmentId = userDepartment?.DepartmentId ?? 0,
                Id = message.Id,
                IsCreation = true,
                IsSubmitForApproval = false
            };

            await thunderCqrs.SendCommand(saveCommand);

            await versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
            {
                VersionSchemaType = VersionSchemaType.DigitalContent,
                ObjectId = message.Id,
                UserId = message.OwnerId,
                ActionComment = $"Created {message.Title}",
                CanRollback = true,
                IncreaseMajorVersion = true,
                IncreaseMinorVersion = false
            });

            if (message.CollaboratorIds != null && message.CollaboratorIds.Any())
            {
                var accessRights = message.CollaboratorIds
                    .Select(id => new AccessRight
                    {
                        Id = Guid.NewGuid(),
                        UserId = id,
                        ObjectId = message.Id,
                        CreatedBy = message.OwnerId,
                    })
                    .ToList();

                await accessRightRepository.InsertManyAsync(accessRights);
            }
        }
    }
}
