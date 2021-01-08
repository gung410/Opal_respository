using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.Microservice.Metadata.Constants;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.created.employee")]
    public class UserCreatedConsumer : ScopedOpalMessageConsumer<UserCreatedMessage>
    {
        public async Task InternalHandleAsync(
            UserCreatedMessage message,
            IReadOnlyRepository<MetadataTag> readMetadataTagRepository,
            SaveUserLogic saveUserLogic)
        {
            var easSubstantiveGradeBandings = readMetadataTagRepository
                .GetAll()
                .Where(x => x.GroupCode == MetadataTagGroupCodes.LearningFxs && x.ParentTagId == MetadataTagConstants.EasSubstantiveGradeBanding)
                .Select(x => x.Id.ToString())
                .ToList();

            await saveUserLogic.SaveCourseUser(
                new SaveCourseUserRequestDto
                {
                    DepartmentId = message.DepartmentId,
                    EmailAddress = message.UserData.EmailAddress,
                    FirstName = message.UserData.FirstName,
                    LastName = message.UserData.LastName,
                    PhoneNumber = message.UserData.MobileNumber,
                    Identity = new SaveCourseUserRequestDtoIdentity { ExtId = message.UserData.Identity.ExtId, Id = message.UserData.Identity.Id },
                    EntityStatus = new SaveCourseUserRequestDtoEntityStatus { ExternallyMastered = message.UserData.EntityStatus.ExternallyMastered, Status = message.UserData.EntityStatus.StatusId },
                    TrackIds = message.UserData.CareerPaths?.Where(x => !string.IsNullOrEmpty(x.Identity.ExtId)).Select(x => x.Identity.ExtId).ToList(),
                    DevelopmentalRoleIds = message.UserData.DevelopmentalRoles?.Where(x => !string.IsNullOrEmpty(x.Identity.ExtId)).Select(x => x.Identity.ExtId).ToList(),
                    ServiceSchemeIds = message.UserData.PersonnelGroups?.Where(x => !string.IsNullOrEmpty(x.Identity.ExtId)).Select(x => x.Identity.ExtId).ToList(),
                    EasSubstantiveGradeBandingIds = message.UserData.LearningFrameworks?.Where(x => !string.IsNullOrEmpty(x.Identity.ExtId) && easSubstantiveGradeBandings.Contains(x.Identity.ExtId)).Select(x => x.Identity.ExtId).ToList(),
                    TeachingLevelIds = message.UserData.JsonDynamicAttributes?.TeachingLevels,
                    JobFamilyIds = message.UserData.JsonDynamicAttributes?.JobFamily,
                    TeachingCourseOfStudyIds = message.UserData.JsonDynamicAttributes?.TeachingCourseOfStudy,
                    TeachingSubjectIds = message.UserData.JsonDynamicAttributes?.TeachingSubjects,
                    CocurricularActivityIds = message.UserData.JsonDynamicAttributes?.CocurricularActivities,
                    LearningFrameworks = message.UserData.LearningFrameworks?.Where(x => !string.IsNullOrEmpty(x.Identity.ExtId)).Select(x => x.Identity.ExtId).ToList(),
                    Designations = string.IsNullOrEmpty(message.UserData.JsonDynamicAttributes?.Designation) ? null : new List<string> { message.UserData.JsonDynamicAttributes?.Designation },
                    PrimaryApprovingOfficerId = message.UserData.Groups?
                            .FirstOrDefault(p => p.Type == CourseUserGroupType.PrimaryApprovalGroup)?.UserIdentity.ExtId ?? Guid.Empty,
                    AlternativeApprovingOfficerId = message.UserData.Groups?
                            .Where(p => p.Type == CourseUserGroupType.AlternativeApprovalGroup)
                            .Select(x => x.UserIdentity.ExtId)
                            .FirstOrDefault(),
                    SystemRoles = message.UserData.SystemRoles?
                             .Where(x => UserRoles.SystemRoleMapping != null && UserRoles.SystemRoleMapping.ContainsKey(x.Identity.ExtId))
                             .Select(x => UserRoles.SystemRoleMapping[x.Identity.ExtId]).ToList()
                });
        }
    }
}
