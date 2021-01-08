using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Mappings
{
    public class ExternalUserGroupMappingService : IUserGroupMappingService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IWorkContext _workContext;
        private readonly IPropertyService _propertyService;

        public ExternalUserGroupMappingService(
            IDepartmentRepository departmentRepository,
            IWorkContext workContext,
            IPropertyService propertyService)
        {
            _departmentRepository = departmentRepository;
            _workContext = workContext;
            _propertyService = propertyService;
        }
        private List<EntityKeyValueDto> GetDynamicProperties(int userGroupId, bool? getDynamicProperties = null)
        {
            return getDynamicProperties == true ?
                 _propertyService.GetDynamicProperties(userGroupId, TableTypes.UserGroup) :
                 new List<EntityKeyValueDto>();
        }
        public ConexusBaseDto ToUserGroupDto(UserGroupEntity groupEntity, bool? getDynamicProperties = null)
        {
            if (groupEntity == null || groupEntity.ArchetypeId != (short)ArchetypeEnum.ExternalUserGroup)
            {
                return null;
            }
            var departmentId = groupEntity.DepartmentId ?? 0;

            DepartmentEntity department = departmentId > 0 
                                          ? _departmentRepository.GetById(departmentId):
                                          new DepartmentEntity();

            if (_workContext.CurrentCustomerId == department.CustomerId)
            {
                var externalUserGroupDto = new ExternalUserGroupDto
                {
                    Identity = new IdentityDto
                    {
                        Archetype = groupEntity.ArchetypeId.HasValue ? (ArchetypeEnum)groupEntity.ArchetypeId : ArchetypeEnum.Unknown,
                        ExtId = groupEntity.ExtId,
                        Id = groupEntity.UserGroupId,
                        OwnerId = groupEntity.OwnerId
                    },
                    Name = groupEntity.Name,
                    Description = groupEntity.Description,
                    Type = groupEntity.UserGroupTypeId.HasValue ? (GrouptypeEnum)groupEntity.UserGroupTypeId : GrouptypeEnum.Default,
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
                    ParentDepartmentId = (int)groupEntity.DepartmentId,
                    ReferrerArchetypeId = groupEntity.ReferrerArchetypeId,
                    ReferrerToken = groupEntity.ReferrerToken,
                    ReferrerResource = groupEntity.ReferrerResource,
                    UserId = groupEntity.UserId
                };
                //GetUserGroupPeriod(externalUserGroupDto, groupEntity);

                return externalUserGroupDto;
            }
            return null;
        }
        public UserGroupEntity ToUserGroupEntity(UserGroupEntity groupEntity, UserGroupDtoBase groupDto)
        {
            var externalUserGroupDto = groupDto as ExternalUserGroupDto;

            if (groupEntity == null)
            {
                groupEntity = new UserGroupEntity()
                {
                    Name = string.Empty,
                    Description = string.Empty,
                    Tag = string.Empty,
                    ExtId = string.Empty,
                    ReferrerToken = string.Empty,
                    ReferrerResource = string.Empty
                    
                };
                groupEntity.Created = DateTime.Now;
            }

            CheckEntityVersion(externalUserGroupDto.EntityStatus.EntityVersion, groupEntity.EntityVersion);
            groupEntity.Name = externalUserGroupDto.Name ?? groupEntity.Name;
            groupEntity.ArchetypeId = (short)ArchetypeEnum.ExternalUserGroup;
            groupEntity.Tag = string.IsNullOrEmpty(externalUserGroupDto.Tag) ? string.Empty : externalUserGroupDto.Tag;
            groupEntity.OwnerId = externalUserGroupDto.Identity.OwnerId;
            groupEntity.DepartmentId = externalUserGroupDto.ParentDepartmentId;
            groupEntity.ExtId = externalUserGroupDto.Identity.ExtId ?? groupEntity.ExtId;
            groupEntity.Description = externalUserGroupDto.Description ?? groupEntity.Description;
            groupEntity.LastUpdated = DateTime.Now;
            groupEntity.EntityStatusId = (short)externalUserGroupDto.EntityStatus.StatusId;
            groupEntity.EntityStatusReasonId = (int)externalUserGroupDto.EntityStatus.StatusReasonId;
            groupEntity.LastUpdatedBy = externalUserGroupDto.EntityStatus.LastUpdatedBy > 0
                                            ? externalUserGroupDto.EntityStatus.LastUpdatedBy
                                            : _workContext.CurrentUserId;
            if (externalUserGroupDto.EntityStatus.LastExternallySynchronized.HasValue)
            {
                groupEntity.LastSynchronized = externalUserGroupDto.EntityStatus.LastExternallySynchronized.Value;
            }
            //SetUserGroupPeriod(groupDto, groupEntity);
            //Default value is zero
            groupEntity.UserGroupTypeId = (int)(groupDto.Type != 0 ? groupDto.Type : GrouptypeEnum.Default);
            groupEntity.ReferrerArchetypeId = externalUserGroupDto.ReferrerArchetypeId ??  groupEntity.ReferrerArchetypeId;
            groupEntity.ReferrerToken = externalUserGroupDto.ReferrerToken ?? groupEntity.ReferrerToken;
            groupEntity.ReferrerResource = externalUserGroupDto.ReferrerResource ?? groupEntity.ReferrerResource;
            groupEntity.UserId = externalUserGroupDto.UserId;
            return groupEntity;
        }
        //TODO move this code to base class
        //private void SetUserGroupPeriod(UserGroupDtoBase groupDto, UserGroupEntity userGroup)
        //{
        //    var periods = _readonlyDataMemoryCache.Periods;
        //    //Get current period 
        //    var currentPeriod = periods.FirstOrDefault(x => x.Startdate <= DateTime.Now && x.Enddate >= DateTime.Now);
        //    if (groupDto.Period == null)
        //    {
        //        userGroup.PeriodId = currentPeriod.PeriodId;
        //        return;
        //    }
        //    var matchPeriod = periods.FirstOrDefault(x => x.ExtId == groupDto.Period.Name);

        //    if (matchPeriod == null)
        //    {
        //        userGroup.PeriodId = currentPeriod.PeriodId;
        //        return;
        //    }
        //    userGroup.PeriodId = matchPeriod.PeriodId;
        //    return;
        //}
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
            DepartmentEntity department = _departmentRepository.GetById(departmentId);

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
