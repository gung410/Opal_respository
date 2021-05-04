using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxEvent.Client;
using cxOrganization.Business.Common;
using cxOrganization.Business.Exceptions;
using cxOrganization.Business.Extensions;
using cxOrganization.Domain;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.ApiClient;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Business.MoveOrganization.MoveDepartment
{
    public class MoveDepartmentService : IMoveDepartmentService
    {
        private readonly ILogger _logger;

        private readonly IAdvancedWorkContext _workContext;
        private readonly OrganizationDbContext _organizationUnitOfWork;
        private readonly IUserService _userService;
        private readonly IEventLogDomainApiClient _eventClientService;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        private readonly MoveDepartmentConfig _moveDepartmentConfig;
        public MoveDepartmentService(IAdvancedWorkContext workContext,
            IUserService userService,
            ILoggerFactory loggerFactory,
            IEventLogDomainApiClient eventClientService,
            OrganizationDbContext organizationUnitOfWork,
            IHierarchyDepartmentService hierarchyDepartmentService,
            IOptions<MoveDepartmentConfig> moveDepartmentConfigOption)
        {
            _workContext = workContext;
            _logger = loggerFactory.CreateLogger<MoveDepartmentService>();
            _userService = userService;
            _organizationUnitOfWork = organizationUnitOfWork;
            _eventClientService = eventClientService;
            _hierarchyDepartmentService = hierarchyDepartmentService;
            _moveDepartmentConfig = moveDepartmentConfigOption.Value;
        }

        public MoveDepartmentsResultDto MoveDepartments(MoveDepartmentsDto moveDepartmentDto)
        {
            ValidateMoveDepartmentsDto(moveDepartmentDto);

            IdentityDto updatedByIdentity = GetValidUpdatedIdentity(moveDepartmentDto);

            var allGivenDepartmentIdentities = new List<IdentityDto>(moveDepartmentDto.Identities) { moveDepartmentDto.TargetParent };
            var existingHierarchyDepartments = GetExistingHierarchyDepartments(allGivenDepartmentIdentities);

            var targetDepartmenHierarchyDepartment = GetValidTargetParentHierarchyDepartment(moveDepartmentDto, existingHierarchyDepartments);

            var moveDepartmentResultDtos = ExecuteMovingDepartments(moveDepartmentDto, existingHierarchyDepartments, targetDepartmenHierarchyDepartment);

            var successfulResults = moveDepartmentResultDtos.Where(r => r.Status.IsOk()).ToList();

            if (successfulResults.Count > 0)
            {
                HandleUserAndEventAfterMovingDepartmentAsync(
                    updatedByIdentity,
                    existingHierarchyDepartments,
                    targetDepartmenHierarchyDepartment,
                    successfulResults,
                    moveDepartmentDto.ForceUserLoginAgain);

            }

            return new MoveDepartmentsResultDto()
            {
                UpdatedByIdentity = updatedByIdentity,
                DepartmentResults = moveDepartmentResultDtos
            };
        }

        private void HandleUserAndEventAfterMovingDepartmentAsync(IdentityDto updatedByIdentity,
            List<HierarchyDepartmentEntity> existingHierarchyDepartments,
            HierarchyDepartmentEntity targetDepartmenHierarchyDepartment,
            List<MoveDepartmentResultDto> successfulResults, bool forceUserLoginAgain)
        {
            Task.Factory.StartNew(() =>
            {
                if (forceUserLoginAgain)
                {
                    ForceUsersLoginAgain(successfulResults, updatedByIdentity);
                }

                InsertDomainEvents(new RequestContext(_workContext), targetDepartmenHierarchyDepartment,
                    successfulResults, updatedByIdentity, existingHierarchyDepartments);

            }).ContinueWith(t =>
            {
                var logMessage =
                    "Unexpected error occurs when handling user and event after moving department.";
                _logger.LogError(logMessage, t.Exception);

            }, TaskContinuationOptions.OnlyOnFaulted);
        }


        private void InsertDomainEvents(RequestContext requestContext,
            HierarchyDepartmentEntity targetDepartmenHierarchyDepartment,
            List<MoveDepartmentResultDto> successfulResults, IdentityDto updatedByIdentity,
            List<HierarchyDepartmentEntity> existingHierarchyDepartments)
        {
            var targetIdentityDto = targetDepartmenHierarchyDepartment.Department.ToIdentityDto();
            var defaultEventTypeName = EventType.MOVED.ToEventTypeName("DEPARTMENT");

            var oldHierarchyDepartmentIds = successfulResults.Select(r => r.OldParentHierarchyDepartmentId).ToList();
            var oldHierarchyDepartments = GetHierarchyDepartmentsByIds(existingHierarchyDepartments,
                targetDepartmenHierarchyDepartment.HierarchyId, oldHierarchyDepartmentIds);

            foreach (var successfulResult in successfulResults)
            {
                var oldParentHierarchyDepartment = oldHierarchyDepartments.FirstOrDefault(h => h.HDId == successfulResult.OldParentHierarchyDepartmentId);

                var movingDepartmentIdentity = successfulResult.Identity;

                var eventInfo = new
                {
                    SourceIdentity = oldParentHierarchyDepartment == null ? null : oldParentHierarchyDepartment.Department.ToIdentityDto(),
                    TargetIdentity = targetIdentityDto,
                    Identity = movingDepartmentIdentity
                };

                var eventType = movingDepartmentIdentity.Archetype == ArchetypeEnum.Unknown
                    ? defaultEventTypeName
                    : EventType.MOVED.ToEventTypeName(movingDepartmentIdentity.Archetype);

                var eventDto = new DomainEventBuilder()
                    .CreatedDate(DateTime.Now)
                    .CreatedBy((int)updatedByIdentity.Id)
                    .InDepartment((int)movingDepartmentIdentity.Id, movingDepartmentIdentity.Archetype)
                    .WithAdditionalInformation(eventInfo)
                    .WithEventTypeName(eventType)
                    .CreateWithRequestContext(requestContext);

                _eventClientService.WriteDomainEvent(eventDto, requestContext);

            }
        }

        private void ForceUsersLoginAgain(List<MoveDepartmentResultDto> successfulResults, IdentityDto updatedByIdentity)
        {
            foreach (var successfulResult in successfulResults)
            {
                var movingDepartment = successfulResult.Identity;

                var allDepatmentIdsInHierarchyDepartment = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelowByHdId(successfulResult.HierarchyDepartmentId);

                var allUsersInHierarchyDepartment = _userService.GetListUsersByDepartmentIds(allDepatmentIdsInHierarchyDepartment);

                foreach (var user in allUsersInHierarchyDepartment)
                {
                    if (user.ForceUserLoginAgain) continue;
                    user.ForceUserLoginAgain = true;
                    user.LastUpdatedBy = (int)updatedByIdentity.Id;
                    user.LastUpdated = DateTime.Now;

                    _userService.UpdateUser(user);
                }

            }

            _organizationUnitOfWork.SaveChanges();
        }

        private List<MoveDepartmentResultDto> ExecuteMovingDepartments(MoveDepartmentsDto moveDepartmentDto,
            List<HierarchyDepartmentEntity> existingHierarchyDepartments, HierarchyDepartmentEntity targetDepartmenHierarchyDepartment)
        {
            var moveDepartmentResultDtos = new List<MoveDepartmentResultDto>();
            var targetDepartmenHierarchyDepartmentPathIds = targetDepartmenHierarchyDepartment.Path.Split('\\');

            foreach (var identity in moveDepartmentDto.Identities)
            {
                moveDepartmentResultDtos.Add(ExecuteMovingDepartment(identity, targetDepartmenHierarchyDepartment,
                    targetDepartmenHierarchyDepartmentPathIds, existingHierarchyDepartments));
            }
            _organizationUnitOfWork.SaveChanges();

            return moveDepartmentResultDtos;
        }

        private MoveDepartmentResultDto ExecuteMovingDepartment(IdentityDto givenDepartmentIdentity,
            HierarchyDepartmentEntity targetDepartmenHierarchyDepartment,
            string[] targetDepartmenHierarchyDepartmentPathIds, List<HierarchyDepartmentEntity> existingHierarchyDepartments)
        {
            var movingDepartmenHierarchy = GetExistingHierarchyDepartment(givenDepartmentIdentity, existingHierarchyDepartments);
            if (movingDepartmenHierarchy == null)
            {
                return MoveDepartmentResultDto.CreateNotFound(givenDepartmentIdentity);
            }

            givenDepartmentIdentity.Id = movingDepartmenHierarchy.DepartmentId;
            givenDepartmentIdentity.ExtId = movingDepartmenHierarchy.Department.ExtId;

            var validateStatus = ValidateTargetHierarchy(targetDepartmenHierarchyDepartment, targetDepartmenHierarchyDepartmentPathIds, givenDepartmentIdentity, movingDepartmenHierarchy);

            if (!validateStatus.IsOk())
            {
                return MoveDepartmentResultDto.Create(givenDepartmentIdentity, validateStatus);
            }

            var oldParentHierarchyDepartmentId = movingDepartmenHierarchy.ParentId;

            movingDepartmenHierarchy.ParentId = targetDepartmenHierarchyDepartment.HDId;
            _hierarchyDepartmentService.UpdateHierarchyDepartment(movingDepartmenHierarchy);

            var moveDepartmentResultDto = MoveDepartmentResultDto.Create(givenDepartmentIdentity, MessageStatus.CreateSuccess());
            moveDepartmentResultDto.OldParentHierarchyDepartmentId = oldParentHierarchyDepartmentId ?? 0;
            moveDepartmentResultDto.HierarchyDepartmentId = movingDepartmenHierarchy.HDId;

            return moveDepartmentResultDto;

        }

        private static MessageStatus ValidateTargetHierarchy(HierarchyDepartmentEntity targetDepartmenHierarchyDepartment, string[] targetDepartmenHierarchyDepartmentPathIds, IdentityDto identity, HierarchyDepartmentEntity departmenHierarchy)
        {
            if (departmenHierarchy.HDId == targetDepartmenHierarchyDepartment.HDId)
            {
                return MessageStatus.CreateInvalid(string.Format("{0} is not allowed to move to itself", identity.ToStringInfo()));
            }

            bool isSameLocation = departmenHierarchy.ParentId == targetDepartmenHierarchyDepartment.HDId;

            if (isSameLocation)
            {
                return MessageStatus.CreateNoContent(string.Format("{0} is not performed moving due to same location", identity.ToStringInfo()));
            }

            bool isBelongTargetPath = targetDepartmenHierarchyDepartmentPathIds.Contains(departmenHierarchy.HDId.ToString());
            if (isBelongTargetPath)
            {
                return MessageStatus.CreateInvalid(string.Format("{0} is not allowed to move to a descendant hierarchy", identity.ToStringInfo()));
            }


            return MessageStatus.CreateSuccess();
        }

        private HierarchyDepartmentEntity GetValidTargetParentHierarchyDepartment(MoveDepartmentsDto moveDepartmentDto,
            List<HierarchyDepartmentEntity> existingHierarchyDepartments)
        {
            var targetDepartmenHierarchyDepartment = GetExistingHierarchyDepartment(moveDepartmentDto.TargetParent, existingHierarchyDepartments);

            if (targetDepartmenHierarchyDepartment == null)
            {
                throw new NotFoundException(string.Format("Target parent {0} is not found", moveDepartmentDto.TargetParent.ToStringInfo()));
            }
            return targetDepartmenHierarchyDepartment;
        }

        private HierarchyDepartmentEntity GetExistingHierarchyDepartment(IdentityDto givenDepartmentIdentity,
            List<HierarchyDepartmentEntity> existingHierarchyDepartments)
        {
            if (givenDepartmentIdentity.Id > 0)
            {
                return existingHierarchyDepartments
                    .FirstOrDefault(h => h.Department.ArchetypeId == (int)givenDepartmentIdentity.Archetype && h.DepartmentId == givenDepartmentIdentity.Id);
            }
            else if (!string.IsNullOrEmpty(givenDepartmentIdentity.ExtId))
            {
                return existingHierarchyDepartments
                    .FirstOrDefault(h => h.Department.ArchetypeId == (int)givenDepartmentIdentity.Archetype && h.Department.ExtId == givenDepartmentIdentity.ExtId);
            }
            return null;
        }


        private List<HierarchyDepartmentEntity> GetExistingHierarchyDepartments(List<IdentityDto> givenDepartmentIdentities)
        {
            var currentHierarchyDepartment = _hierarchyDepartmentService.GetCurrentHierarchyDepartment();
            if (currentHierarchyDepartment == null)
            {
                throw new NotFoundException("Current hierarchy department is not found");
            }

            var customerIds = new List<int?> { _workContext.CurrentCustomerId };
            var departmentIdentitiesHaveId = givenDepartmentIdentities.Where(d => d.Id > 0).ToList();
            var departmentIdentitiesHaveExtId = givenDepartmentIdentities
                .Except(departmentIdentitiesHaveId)
                .Where(d => !string.IsNullOrEmpty(d.ExtId)).ToList();

            var existingHierarchyDepartmentEntities = GetHierachyByDepartmentDepartmentByIds(departmentIdentitiesHaveId, currentHierarchyDepartment.HierarchyId, customerIds);
            existingHierarchyDepartmentEntities.AddRange(GetHierachyByDepartmentByDepartmentExtIds(departmentIdentitiesHaveExtId, currentHierarchyDepartment.HierarchyId, customerIds));

            return existingHierarchyDepartmentEntities;
        }

        private List<HierarchyDepartmentEntity> GetHierachyByDepartmentByDepartmentExtIds(List<IdentityDto> departmentIdentitiesHaveExtId, int hirachyId, List<int?> customerIds)
        {
            if (departmentIdentitiesHaveExtId.Count > 0)
            {
                var departmentExtIds = departmentIdentitiesHaveExtId.Select(d => d.ExtId).ToList();
                var departmentArchetypes = departmentIdentitiesHaveExtId.Select(d => d.Archetype).Distinct().ToList();

                return _hierarchyDepartmentService.GetHierarchyDepartmentEntities(_workContext.CurrentOwnerId,
                    hirachyId, customerIds: customerIds, departmentExtIds: departmentExtIds, departmentArchetypes: departmentArchetypes, includeDepartment: true);
            }
            return new List<HierarchyDepartmentEntity>();
        }

        private List<HierarchyDepartmentEntity> GetHierachyByDepartmentDepartmentByIds(List<IdentityDto> departmentIdentitiesHaveId, int hirachyId, List<int?> customerIds)
        {
            if (departmentIdentitiesHaveId.Count > 0)
            {
                var departmentIds = departmentIdentitiesHaveId.Select(d => (int)d.Id).ToList();
                var departmentArchetypes = departmentIdentitiesHaveId.Select(d => d.Archetype).Distinct().ToList();

                return _hierarchyDepartmentService.GetHierarchyDepartmentEntities(_workContext.CurrentOwnerId,
                    hirachyId, customerIds: customerIds, departmentIds: departmentIds, departmentArchetypes: departmentArchetypes, includeDepartment: true);
            }

            return new List<HierarchyDepartmentEntity>();
        }

        private void ValidateMoveDepartmentsDto(MoveDepartmentsDto moveDepartmentDto)
        {
            if (moveDepartmentDto.Identities.IsNullOrEmpty())
                throw new InvalidException("Identities is null or empty");

        }

        private IdentityDto GetValidUpdatedIdentity(MoveDepartmentsDto moveDepartmentsDto)
        {
            IdentityDto updatedIdentityDto;
            if (moveDepartmentsDto.UpdatedByIdentity != null)
            {
                var updatedIdentityStatusDto = GetExistingUser<IdentityStatusDto>(_userService, moveDepartmentsDto.UpdatedByIdentity, null);
                if (updatedIdentityStatusDto == null)
                {
                    throw new NotFoundException(string.Format("UpdatedByIdentity {0} is not found", moveDepartmentsDto.UpdatedByIdentity.ToStringInfo()));
                }
                updatedIdentityDto = updatedIdentityStatusDto.Identity;

            }
            else
            {
                updatedIdentityDto = new IdentityDto
                {
                    OwnerId = _workContext.CurrentOwnerId,
                    CustomerId = _workContext.CurrentCustomerId,
                    Id = _workContext.CurrentUserId,
                    //TODO
                    //Archetype = _workContext.CurrentUser != null ? _workContext.CurrentUser.Archetype : ArchetypeEnum.Unknown

                };
            }

            if (_moveDepartmentConfig.MoveDepartmentByRoles.IsNotNullOrEmpty())
            {
                var existingRoleMemberships = _userService.GetUserMemberships((int)updatedIdentityDto.Id.Value,
                    updatedIdentityDto.Archetype,
                    membershipsArchetypeIds: new List<ArchetypeEnum> { ArchetypeEnum.Role },
                    membershipExtIds: _moveDepartmentConfig.MoveDepartmentByRoles);

                if (existingRoleMemberships.Count == 0)
                {
                    throw new InvalidException(string.Format("{0} is required to have role in list '{1}' to execute moving department",
                        updatedIdentityDto.ToStringInfo(), string.Join(",", _moveDepartmentConfig.MoveDepartmentByRoles)));
                }
            }
            return updatedIdentityDto;
        }

        private T GetExistingUser<T>(IUserService userService, IdentityDto userIdentity,
            List<EntityStatusEnum> statusEnums) where T : ConexusBaseDto
        {

            if (userIdentity.Id > 0)
            {
                return userService.GetUsers<T>(userIdentity.OwnerId,
                        new List<int> { (int)userIdentity.CustomerId },
                        userIds: new List<int> { (int)userIdentity.Id.Value },
                        archetypeIds: new List<ArchetypeEnum> { userIdentity.Archetype },
                        statusIds: statusEnums)
                    .Items.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(userIdentity.ExtId))
            {
                return userService.GetUsers<T>(_workContext.CurrentOwnerId,
                        new List<int> { (int)userIdentity.CustomerId },
                        extIds: new List<string> { userIdentity.ExtId },
                        archetypeIds: new List<ArchetypeEnum> { userIdentity.Archetype },
                        statusIds: statusEnums)
                    .Items.FirstOrDefault();
            }
            return null;
        }
        private List<HierarchyDepartmentEntity> GetHierarchyDepartmentsByIds(List<HierarchyDepartmentEntity> existingHierarchyDepartments, int hirachyId, List<int> hdIds)
        {
            //Try getting hierarchy departments from what we have in memory
            var existingHds = existingHierarchyDepartments.Where(e => hdIds.Contains(e.HDId)).ToList();

            var nonExistingHdIds = hdIds.Where(hdid => !existingHds.Any(h => h.HDId == hdid)).ToList();

            if (nonExistingHdIds.Count > 0)
            {
                //Get more hierarchy departments from db
                existingHds.AddRange(_hierarchyDepartmentService.GetHierarchyDepartmentEntities(_workContext.CurrentOwnerId,
                    hirachyId, hdIds: nonExistingHdIds, includeDepartment: true));
            }

            return existingHds;
        }



    }
}