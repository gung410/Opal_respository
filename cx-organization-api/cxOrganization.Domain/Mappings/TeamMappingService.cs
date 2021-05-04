using System;
using System.Collections.Generic;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Mappings
{
    public class TeamMappingService : IUserGroupMappingService
    {
        private readonly IDepartmentService _departmentService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IPropertyService _propertyService;
        public TeamMappingService(
            IDepartmentService departmentService,
            IAdvancedWorkContext workContext,
            IPropertyService propertyService)
        {
            _departmentService = departmentService;
            _workContext = workContext;
            _propertyService = propertyService;
        }
        private List<EntityKeyValueDto> GetDynamicProperties(int userGroupId, bool? getDynamicProperties = null)
        {
            return getDynamicProperties.HasValue && getDynamicProperties.Value ?
                 _propertyService.GetDynamicProperties(userGroupId, TableTypes.UserGroup) :
                 new List<EntityKeyValueDto>();
        }
        public IdentityStatusDto ToIdentityStatusDto(UserGroupEntity department)
        {
            throw new NotImplementedException();
        }

        public MemberDto ToMemberDto(UserGroupEntity userGroup)
        {
            throw new NotImplementedException();
        }

        public ConexusBaseDto ToUserGroupDto(UserGroupEntity groupEntity, bool? getDynamicProperties = null)
        {
            if (groupEntity == null || groupEntity.ArchetypeId != (short)ArchetypeEnum.Team || !groupEntity.UserGroupTypeId.HasValue)
            {
                return null;
            }
            var departmentId = groupEntity.DepartmentId ?? 0;
            DepartmentEntity department = _departmentService.GetDepartmentById(departmentId);
            if (_workContext.CurrentCustomerId == department.CustomerId)
            {
                var candidatePoolDto = new TeamDto
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
                    Type = (GrouptypeEnum)(groupEntity.UserGroupTypeId.HasValue ? groupEntity.UserGroupTypeId.Value : 1),
                    EntityStatus = new EntityStatusDto
                    {
                        EntityVersion = groupEntity.EntityVersion,
                        LastUpdated = groupEntity.LastUpdated,
                        LastUpdatedBy = groupEntity.LastUpdatedBy ?? 0,
                        StatusId = groupEntity.EntityStatusId.HasValue ? (EntityStatusEnum)groupEntity.EntityStatusId : EntityStatusEnum.Unknown,
                        StatusReasonId = groupEntity.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)groupEntity.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                        LastExternallySynchronized = groupEntity.LastSynchronized,
                        //ExternallyMastered = groupEntity.Locked == 1,
                        Deleted = groupEntity.Deleted.HasValue
                    },
                    ParentDepartmentId = (int)groupEntity.DepartmentId
                };
                //this.Log("ToUserGroupDto").Debug(candidatePoolDto.ToLogString());

                return candidatePoolDto;
            }
            return null;
        }
        public UserGroupEntity ToUserGroupEntity(UserGroupEntity groupEntity, UserGroupDtoBase groupDto, IAdvancedWorkContext workContext = null)
        {
            throw new NotImplementedException();
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
