using System;
using System.Collections;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public class UserGroupMappingService : IUserGroupMappingService
    {
        private readonly IDepartmentService _departmentService;
        
        public UserGroupMappingService(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        public IdentityStatusDto ToIdentityStatusDto(UserGroupEntity userGroupEntity)
        {
            throw new NotImplementedException();
        }

        public MemberDto ToMemberDto(UserGroupEntity userGroup)
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

        public ConexusBaseDto ToUserGroupDto(UserGroupEntity userGroupEntity, bool? getDynamicProperties = null)
        {
            if (!userGroupEntity.ArchetypeId.HasValue) return null;

            return new IdentityStatusDto
            {
                Identity = ToIdentityDto(userGroupEntity),
                EntityStatus = ToEntityStatusDto(userGroupEntity)
            };
        }
        protected void CheckEntityVersion(byte[] clientVersion, byte[] dbVersion)
        {
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(clientVersion, dbVersion))
            {
                throw new Exception(MappingErrorDefault.ERROR_ENTITY_VERSION_INCORRECTED);
            }
        }
        public UserGroupEntity ToUserGroupEntity(UserGroupEntity groupEntity, UserGroupDtoBase groupDto)
        {
            if (groupEntity == null)
                return null;
            CheckEntityVersion(groupDto.EntityStatus.EntityVersion, groupEntity.EntityVersion);
            groupEntity.Name = groupDto.Name ?? groupEntity.Name;
            if (groupEntity.UserGroupTypeId.HasValue)
                groupEntity.UserGroupTypeId = (int)groupDto.Type;
            groupEntity.OwnerId = groupDto.Identity.OwnerId;
            groupEntity.ExtId = groupDto.Identity.ExtId ?? groupEntity.ExtId;
            groupEntity.Description = groupDto.Description ?? groupEntity.Description;
            groupEntity.LastUpdated = DateTime.Now;
            groupEntity.EntityStatusId = (short)groupDto.EntityStatus.StatusId;
            groupEntity.EntityStatusReasonId = (int)groupDto.EntityStatus.StatusReasonId;
            groupEntity.LastUpdatedBy = groupDto.EntityStatus.LastUpdatedBy;
            if (groupDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                groupEntity.LastSynchronized = groupDto.EntityStatus.LastExternallySynchronized.Value;
            }
            groupEntity.UserGroupTypeId = groupEntity.UserGroupTypeId == 0 ? (int)GrouptypeEnum.Default : groupEntity.UserGroupTypeId;
            groupEntity.ReferrerArchetypeId = groupDto.ReferrerArchetypeId.HasValue ? groupDto.ReferrerArchetypeId : groupEntity.ReferrerArchetypeId;
            groupEntity.ReferrerToken = groupDto.ReferrerToken ?? groupEntity.ReferrerToken;
            groupEntity.ReferrerResource = groupDto.ReferrerResource ?? groupEntity.ReferrerResource;
            return groupEntity;
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
                //ExternallyMastered = department.Locked == 1,
                Deleted = userGroup.Deleted.HasValue
            };
        }

        public TeachingSubjectDto ToTeachingSubjectDto(UserGroupEntity userGroupEntity)
        {
            if (!userGroupEntity.ArchetypeId.HasValue) return null;
            var result = new TeachingSubjectDto
            {
                DepartmentId = userGroupEntity.DepartmentId,
                ArchetypeId = userGroupEntity.ArchetypeId,
                Name = userGroupEntity.Name,
                Description = userGroupEntity.Description,
                Identity = ToIdentityDto(userGroupEntity),
                EntityStatus = ToEntityStatusDto(userGroupEntity)
            };
            return result;
        }
    }
}
