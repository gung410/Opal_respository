using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using System;
using System.Collections;

namespace cxOrganization.Domain.Mappings
{
    public class UserPoolMappingService : IUserGroupMappingService
    {
        private readonly IDepartmentService _departmentService;
        private readonly IWorkContext _workContext;

        public UserPoolMappingService(
            IDepartmentService departmentService,
            IWorkContext workContext,
            IPropertyService propertyService)
        {
            _departmentService = departmentService;
            _workContext = workContext;
        }
        public ConexusBaseDto ToUserGroupDto(UserGroupEntity groupEntity, bool? getDynamicProperties = null)
        {
            if (groupEntity == null || (groupEntity.ArchetypeId != (short)ArchetypeEnum.UserPool) || !groupEntity.UserGroupTypeId.HasValue)
            {
                return null;
            }
            var departmentId = groupEntity.DepartmentId ?? 0;
            DepartmentEntity department = _departmentService.GetDepartmentById(departmentId);

            var userPoolDto = new UserPoolDto
            {
                Identity = new IdentityDto
                {
                    Archetype = groupEntity.ArchetypeId.HasValue ? (ArchetypeEnum)groupEntity.ArchetypeId : ArchetypeEnum.Unknown,
                    ExtId = groupEntity.ExtId,
                    Id = groupEntity.UserGroupId,
                    CustomerId = department.CustomerId ?? 0,
                    OwnerId = groupEntity.OwnerId
                },
                Name = groupEntity.Name,
                Description = groupEntity.Description,
                Type = (GrouptypeEnum)(groupEntity.UserGroupTypeId.HasValue ? groupEntity.UserGroupTypeId.Value : (int)GrouptypeEnum.Default),
                EntityStatus = new EntityStatusDto
                {
                    EntityVersion = groupEntity.EntityVersion,
                    LastUpdated = groupEntity.LastUpdated,
                    LastUpdatedBy = groupEntity.LastUpdatedBy ?? 0,
                    StatusId = groupEntity.EntityStatusId.HasValue ? (EntityStatusEnum)groupEntity.EntityStatusId : EntityStatusEnum.Unknown,
                    StatusReasonId = groupEntity.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)groupEntity.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                    LastExternallySynchronized = groupEntity.LastSynchronized,
                    Deleted = groupEntity.Deleted.HasValue
                },
                DepartmentId = groupEntity.DepartmentId,
                UserId = groupEntity.UserId,
                ReferrerArchetypeId = groupEntity.ReferrerArchetypeId,
                ReferrerToken = groupEntity.ReferrerToken,
                ReferrerResource = groupEntity.ReferrerResource
            };

            return userPoolDto;
        }

        public UserGroupEntity ToUserGroupEntity(UserGroupEntity groupEntity, UserGroupDtoBase groupDto)
        {
            var userPool = groupDto as UserPoolDto;

            userPool.EntityStatus.LastUpdatedBy = userPool.EntityStatus.LastUpdatedBy > 0
                                                             ? userPool.EntityStatus.LastUpdatedBy
                                                             : _workContext.CurrentUserId;
            if (groupDto.Identity.Id == null || groupDto.Identity.Id <= 0)
            {
                var userGroup = new UserGroupEntity
                {
                    ArchetypeId = (int)groupDto.Identity.Archetype,
                    Name = userPool.Name ?? string.Empty,
                    UserGroupTypeId = (int)(groupDto.Type > 0 ? groupDto.Type : GrouptypeEnum.Default),
                    UserId = userPool.UserId,
                    Description = userPool.Description ?? string.Empty,
                    OwnerId = userPool.Identity.OwnerId,
                    DepartmentId = userPool.DepartmentId,
                    ExtId = userPool.Identity.ExtId ?? string.Empty,
                    EntityStatusId = userPool.EntityStatus.StatusId == EntityStatusEnum.Unknown ? (int)EntityStatusEnum.Inactive : (int)userPool.EntityStatus.StatusId,
                    EntityStatusReasonId = (int)userPool.EntityStatus.StatusReasonId,
                    LastUpdatedBy = userPool.EntityStatus.LastUpdatedBy,
                    Created = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    ReferrerArchetypeId = userPool.ReferrerArchetypeId,
                    ReferrerToken = userPool.ReferrerToken ?? string.Empty,
                    ReferrerResource = userPool.ReferrerResource ?? string.Empty
                };
                if (userPool.EntityStatus.LastExternallySynchronized.HasValue)
                {
                    userGroup.LastSynchronized = userPool.EntityStatus.LastExternallySynchronized.Value;
                }
                return userGroup;
            }
            else
            {
                if (groupEntity == null)
                    return null;
                CheckEntityVersion(userPool.EntityStatus.EntityVersion, groupEntity.EntityVersion);
                groupEntity.Name = userPool.Name ?? groupEntity.Name;
                if (groupEntity.UserGroupTypeId.HasValue)
                    groupEntity.UserGroupTypeId = (int)groupDto.Type;
                groupEntity.OwnerId = userPool.Identity.OwnerId;
                groupEntity.DepartmentId = userPool.DepartmentId;
                groupEntity.UserId = userPool.UserId;
                groupEntity.ExtId = userPool.Identity.ExtId ?? groupEntity.ExtId;
                groupEntity.Description = userPool.Description ?? groupEntity.Description;
                groupEntity.LastUpdated = DateTime.Now;
                groupEntity.EntityStatusId = (short)userPool.EntityStatus.StatusId;
                groupEntity.EntityStatusReasonId = (int)userPool.EntityStatus.StatusReasonId;
                groupEntity.LastUpdatedBy = userPool.EntityStatus.LastUpdatedBy;
                if (userPool.EntityStatus.LastExternallySynchronized.HasValue)
                {
                    groupEntity.LastSynchronized = userPool.EntityStatus.LastExternallySynchronized.Value;
                }
                groupEntity.UserGroupTypeId = groupEntity.UserGroupTypeId == 0 ? (int)GrouptypeEnum.Default : groupEntity.UserGroupTypeId;
                groupEntity.ReferrerArchetypeId = userPool.ReferrerArchetypeId.HasValue ? userPool.ReferrerArchetypeId : groupEntity.ReferrerArchetypeId;
                groupEntity.ReferrerToken = userPool.ReferrerToken ?? groupEntity.ReferrerToken;
                groupEntity.ReferrerResource = userPool.ReferrerResource ?? groupEntity.ReferrerResource;
                return groupEntity;
            }
        }

        public virtual IdentityStatusDto ToIdentityStatusDto(UserGroupEntity userGroup)
        {
            //Don't map Entity that have Archetype is NULL
            if (!userGroup.ArchetypeId.HasValue) return null;

            return new IdentityStatusDto
            {
                Identity = ToIdentityDto(userGroup),
                EntityStatus = ToEntityStatusDto(userGroup)
            };
        }

        public virtual MemberDto ToMemberDto(UserGroupEntity userGroup)
        {
            if (!userGroup.ArchetypeId.HasValue)
                return null;
            return new MemberDto
            {
                Identity = ToIdentityDto(userGroup),
                EntityStatus = ToEntityStatusDto(userGroup),
                Role = string.Empty
            };
        }

        protected IdentityDto ToIdentityDto(UserGroupEntity groupEntity)
        {
            var departmentId = groupEntity.DepartmentId ?? 0;
            DepartmentEntity department = _departmentService.GetDepartmentById(departmentId);

            return new IdentityDto
            {
                Id = groupEntity.UserGroupId,
                Archetype = groupEntity.ArchetypeId.HasValue ? (ArchetypeEnum)groupEntity.ArchetypeId : ArchetypeEnum.Unknown,
                CustomerId = department.CustomerId ?? 0,
                ExtId = groupEntity.ExtId,
                OwnerId = groupEntity.OwnerId
            };
        }

        protected EntityStatusDto ToEntityStatusDto(UserGroupEntity userGroup)
        {
            return new EntityStatusDto
            {
                EntityVersion = userGroup.EntityVersion,
                LastUpdated = userGroup.LastUpdated,
                LastUpdatedBy = userGroup.LastUpdatedBy ?? 0,
                StatusId = userGroup.EntityStatusId.HasValue ? (EntityStatusEnum)userGroup.EntityStatusId : EntityStatusEnum.Unknown,
                StatusReasonId = userGroup.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)userGroup.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                LastExternallySynchronized = userGroup.LastSynchronized,
                Deleted = userGroup.Deleted.HasValue
            };
        }

        protected void CheckEntityVersion(byte[] clientVersion, byte[] dbVersion)
        {
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(clientVersion, dbVersion))
            {
                throw new Exception(MappingErrorDefault.ERROR_ENTITY_VERSION_INCORRECTED);
            }
        }

        public TeachingSubjectDto ToTeachingSubjectDto(UserGroupEntity userGroupEntity)
        {
            if (!userGroupEntity.ArchetypeId.HasValue) return null;
            var result = new TeachingSubjectDto
            {
                DepartmentId = userGroupEntity.DepartmentId,
                ArchetypeId = userGroupEntity.ArchetypeId
            };
            return result;
        }
    }
}
