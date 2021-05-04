using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class TeachingGroupMappingService : IUserGroupMappingService
    {
        private readonly IDepartmentService _departmentService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IPropertyService _propertyService;
        public TeachingGroupMappingService(
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
        public ConexusBaseDto ToUserGroupDto(UserGroupEntity groupEntity, bool? getDynamicProperties = null)
        {
            if (groupEntity == null || (groupEntity.ArchetypeId != (short)ArchetypeEnum.TeachingGroup && groupEntity.ArchetypeId != (short)ArchetypeEnum.EducationProgram) || !groupEntity.UserGroupTypeId.HasValue)
            {
                return null;
            }
            var departmentId = groupEntity.DepartmentId ?? 0;
            DepartmentEntity department = _departmentService.GetDepartmentById(departmentId);

            var teachingGroupDto = new TeachingGroupDto
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
                //SubjectUiId = groupEntity.Description,
                SubjectCode = groupEntity.Tag,
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
                SchoolId = groupEntity.DepartmentId,
                ReferrerArchetypeId = groupEntity.ReferrerArchetypeId,
                ReferrerToken = groupEntity.ReferrerToken,
                ReferrerResource = groupEntity.ReferrerResource
            };

            GetUserGroupPeriod(teachingGroupDto, groupEntity);

            return teachingGroupDto;
        }

        public UserGroupEntity ToUserGroupEntity(UserGroupEntity groupEntity, UserGroupDtoBase groupDto, IAdvancedWorkContext workContext = null)
        {
            var teachingGroup = groupDto as TeachingGroupDto;

            teachingGroup.EntityStatus.LastUpdatedBy = teachingGroup.EntityStatus.LastUpdatedBy > 0
                                                             ? teachingGroup.EntityStatus.LastUpdatedBy
                                                             : _workContext.CurrentUserId;
            if (groupDto.Identity.Id == 0)
            {
                var userGroup = new UserGroupEntity
                {
                    ArchetypeId = (int)groupDto.Identity.Archetype,
                    Name = teachingGroup.Name ?? string.Empty,
                    UserGroupTypeId = (int)teachingGroup.Type,
                    Description = teachingGroup.Description ?? string.Empty,
                    //Tag = teachingGroup.SubjectUiId,
                    Tag = teachingGroup.SubjectCode ?? string.Empty,
                    OwnerId = teachingGroup.Identity.OwnerId,
                    DepartmentId = teachingGroup.SchoolId,
                    ExtId = teachingGroup.Identity.ExtId ?? string.Empty,
                    EntityStatusId = teachingGroup.EntityStatus.StatusId == EntityStatusEnum.Unknown ? (int)EntityStatusEnum.Inactive : (int)teachingGroup.EntityStatus.StatusId,
                    EntityStatusReasonId = (int)teachingGroup.EntityStatus.StatusReasonId,
                    LastUpdatedBy = teachingGroup.EntityStatus.LastUpdatedBy,
                    Created = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    ReferrerArchetypeId = teachingGroup.ReferrerArchetypeId,
                    ReferrerToken = teachingGroup.ReferrerToken ?? string.Empty,
                    ReferrerResource = teachingGroup.ReferrerResource ?? string.Empty
                };
                if (teachingGroup.EntityStatus.LastExternallySynchronized.HasValue)
                {
                    userGroup.LastSynchronized = teachingGroup.EntityStatus.LastExternallySynchronized.Value;
                }
                SetUserGroupPeriod(groupDto, userGroup);
                userGroup.UserGroupTypeId = userGroup.UserGroupTypeId == 0 ? (int)GrouptypeEnum.Default : userGroup.UserGroupTypeId;
                return userGroup;
            }
            else
            {
                if (groupEntity == null)
                    return null;
                CheckEntityVersion(teachingGroup.EntityStatus.EntityVersion, groupEntity.EntityVersion);
                groupEntity.Name = teachingGroup.Name ?? groupEntity.Name;
                if (groupEntity.UserGroupTypeId.HasValue)
                    groupEntity.UserGroupTypeId = (int)groupDto.Type;
                groupEntity.OwnerId = teachingGroup.Identity.OwnerId;
                groupEntity.DepartmentId = teachingGroup.SchoolId;
                groupEntity.Tag = teachingGroup.SubjectCode ?? groupEntity.Tag;
                groupEntity.ExtId = teachingGroup.Identity.ExtId ?? groupEntity.ExtId;
                groupEntity.Description = teachingGroup.Description ?? groupEntity.Description;
                groupEntity.LastUpdated = DateTime.Now;
                groupEntity.EntityStatusId = (short)teachingGroup.EntityStatus.StatusId;
                groupEntity.EntityStatusReasonId = (int)teachingGroup.EntityStatus.StatusReasonId;
                groupEntity.LastUpdatedBy = teachingGroup.EntityStatus.LastUpdatedBy;
                if (teachingGroup.EntityStatus.LastExternallySynchronized.HasValue)
                {
                    groupEntity.LastSynchronized = teachingGroup.EntityStatus.LastExternallySynchronized.Value;
                }
                //userGroup.Locked = (short)(teachingGroup.EntityStatus.ExternallyMastered ? 1 : 0);
                SetUserGroupPeriod(groupDto, groupEntity);
                groupEntity.UserGroupTypeId = groupEntity.UserGroupTypeId == 0 ? (int)GrouptypeEnum.Default : groupEntity.UserGroupTypeId;
                groupEntity.ReferrerArchetypeId = teachingGroup.ReferrerArchetypeId.HasValue ? teachingGroup.ReferrerArchetypeId : groupEntity.ReferrerArchetypeId;
                groupEntity.ReferrerToken = teachingGroup.ReferrerToken ?? groupEntity.ReferrerToken;
                groupEntity.ReferrerResource = teachingGroup.ReferrerResource ?? groupEntity.ReferrerResource;
                return groupEntity;
            }
        }

        private void SetUserGroupPeriod(UserGroupDtoBase groupDto, UserGroupEntity userGroup)
        {
            //TODO uncomment the code bellow
            //var periods = _readonlyDataMemoryCache.Periods;
            ////Get current period 
            //var currentPeriod = periods.FirstOrDefault(x => x.Startdate <= DateTime.Now && x.Enddate >= DateTime.Now);
            //if (groupDto.Period == null)
            //{
            //    if (currentPeriod != null)
            //    {
            //        userGroup.PeriodId = currentPeriod.PeriodId;
            //    }
            //    return;
            //}
            //var matchPeriod = periods.FirstOrDefault(x => x.ExtId == groupDto.Period.Name);

            //if (matchPeriod != null)
            //{
            //    userGroup.PeriodId = matchPeriod.PeriodId;
            //}
            //else if (currentPeriod != null)
            //{
            //    userGroup.PeriodId = currentPeriod.PeriodId;
            //}
        }

        private void GetUserGroupPeriod(UserGroupDtoBase groupDto, UserGroupEntity userGroup)
        {
            if (!userGroup.PeriodId.HasValue)
                return;
            //TODO uncomment the code bellow
            //var period = _readonlyDataMemoryCache.Periods.FirstOrDefault(x => x.PeriodId == userGroup.PeriodId);
            //if (period == null)
            //    return;
            //groupDto.Period = new PeriodDto
            //{
            //    StartDate = period.Startdate,
            //    EndDate = period.Enddate,
            //    Name = period.ExtID
            //};

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
