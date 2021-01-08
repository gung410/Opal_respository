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
    public class CandidatePoolMappingService : IUserGroupMappingService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;

        private readonly IWorkContext _workContext;
        private readonly IPropertyService _propertyService;

        public CandidatePoolMappingService(IDepartmentRepository departmentRepository,
            IWorkContext workContext,
            IPropertyService propertyService, 
            IUserRepository userRepository)
        {
            _departmentRepository = departmentRepository;
            _workContext = workContext;
            _propertyService = propertyService;
            _userRepository = userRepository;
        }
        private List<EntityKeyValueDto> GetDynamicProperties(int userGroupId, bool? getDynamicProperties = null)
        {
            return getDynamicProperties.HasValue && getDynamicProperties.Value ?
                 _propertyService.GetDynamicProperties(userGroupId, TableTypes.UserGroup) :
                 new List<EntityKeyValueDto>();
        }
        public ConexusBaseDto ToUserGroupDto(UserGroupEntity groupEntity, bool? getDynamicProperties = null)
        {
            if (groupEntity == null || groupEntity.ArchetypeId != (short)ArchetypeEnum.CandidatePool || !groupEntity.UserGroupTypeId.HasValue)
            {
                return null;
            }
            int customerId = GetCustomerId(groupEntity);
            if (_workContext.CurrentCustomerId == customerId)
            {
                var candidatePoolDto = new CandidatePoolDto
                {
                    Identity = new IdentityDto
                    {
                        Archetype = groupEntity.ArchetypeId.HasValue ? (ArchetypeEnum)groupEntity.ArchetypeId : ArchetypeEnum.Unknown,
                        ExtId = groupEntity.ExtId,
                        Id = groupEntity.UserGroupId,
                        CustomerId = customerId,
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
                    ParentDepartmentId = groupEntity.DepartmentId,
                    ParentUserId = groupEntity.UserId,
                    ReferrerArchetypeId = groupEntity.ReferrerArchetypeId,
                    ReferrerToken = groupEntity.ReferrerToken,
                    ReferrerResource = groupEntity.ReferrerResource
                };
                //TODO
                //GetUserGroupPeriod(candidatePoolDto, groupEntity);
                
                return candidatePoolDto;
            }
            return null;


        }

        private int GetCustomerId(UserGroupEntity groupEntity)
        {
            var customerId = 0;
            if (groupEntity.DepartmentId > 0)
            {
                var department = groupEntity.Department ?? _departmentRepository.GetById(groupEntity.DepartmentId.Value);
                if (department != null)
                    customerId = department.CustomerId ?? 0;
            }
            if (customerId == 0 && groupEntity.UserId > 0)
            {
                var user = _userRepository.GetById(groupEntity.UserId.Value);
                if (user != null)
                    customerId = user.CustomerId ?? 0;
            }

            return customerId;
        }

        public UserGroupEntity ToUserGroupEntity(UserGroupEntity groupEntity, UserGroupDtoBase groupDto)
        {
            var candidatePoolDto = groupDto as CandidatePoolDto;
            if (groupDto.Identity.Id == null || groupDto.Identity.Id <= 0)
            {
                var userGroup = new UserGroupEntity
                {
                    ArchetypeId = (int)ArchetypeEnum.CandidatePool,
                    Name = candidatePoolDto.Name ?? string.Empty,
                    UserGroupTypeId = (int)candidatePoolDto.Type,
                    Description = candidatePoolDto.Description ?? string.Empty,
                    Tag = "",
                    OwnerId = candidatePoolDto.Identity.OwnerId,
                    DepartmentId = candidatePoolDto.ParentDepartmentId>0?candidatePoolDto.ParentDepartmentId:null,
                    UserId = candidatePoolDto.ParentUserId>0?candidatePoolDto.ParentUserId:null,
                    ExtId = candidatePoolDto.Identity.ExtId ?? string.Empty,
                    EntityStatusId = candidatePoolDto.EntityStatus.StatusId == EntityStatusEnum.Unknown ? (int)EntityStatusEnum.Inactive : (int)candidatePoolDto.EntityStatus.StatusId,
                    EntityStatusReasonId = (int)candidatePoolDto.EntityStatus.StatusReasonId,
                    LastUpdatedBy = candidatePoolDto.EntityStatus.LastUpdatedBy > 0
                        ? candidatePoolDto.EntityStatus.LastUpdatedBy
                        : _workContext.CurrentUserId,
                    Created = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    ReferrerArchetypeId = candidatePoolDto.ReferrerArchetypeId,
                    ReferrerToken = candidatePoolDto.ReferrerToken ?? string.Empty,
                    ReferrerResource = candidatePoolDto.ReferrerResource ?? string.Empty
                };
                if (candidatePoolDto.EntityStatus.LastExternallySynchronized.HasValue)
                {
                    userGroup.LastSynchronized = candidatePoolDto.EntityStatus.LastExternallySynchronized.Value;
                }
                //TODO
                //SetUserGroupPeriod(groupDto, userGroup);
                userGroup.UserGroupTypeId = userGroup.UserGroupTypeId == 0 ? (int)GrouptypeEnum.Default : userGroup.UserGroupTypeId;
                return userGroup;
            }
            else
            {
                if (groupEntity == null)
                    return null;
                CheckEntityVersion(candidatePoolDto.EntityStatus.EntityVersion, groupEntity.EntityVersion);
                groupEntity.Name = candidatePoolDto.Name ?? groupEntity.Name;
                if (groupEntity.UserGroupTypeId.HasValue)
                    groupEntity.UserGroupTypeId = (int)groupDto.Type;
                groupEntity.OwnerId = candidatePoolDto.Identity.OwnerId;
                groupEntity.DepartmentId = candidatePoolDto.ParentDepartmentId > 0 ? candidatePoolDto.ParentDepartmentId : null;
                groupEntity.UserId = candidatePoolDto.ParentUserId > 0 ? candidatePoolDto.ParentUserId : null;
                groupEntity.ExtId = candidatePoolDto.Identity.ExtId ?? groupEntity.ExtId;
                groupEntity.Description = candidatePoolDto.Description ?? groupEntity.Description;
                groupEntity.LastUpdated = DateTime.Now;
                groupEntity.EntityStatusId = (short)candidatePoolDto.EntityStatus.StatusId;
                groupEntity.EntityStatusReasonId = (int)candidatePoolDto.EntityStatus.StatusReasonId;
                groupEntity.LastUpdatedBy = candidatePoolDto.EntityStatus.LastUpdatedBy > 0
                    ? candidatePoolDto.EntityStatus.LastUpdatedBy
                    : _workContext.CurrentUserId;
                if (candidatePoolDto.EntityStatus.LastExternallySynchronized.HasValue)
                {
                    groupEntity.LastSynchronized = candidatePoolDto.EntityStatus.LastExternallySynchronized.Value;
                }
                //userGroup.Locked = (short)(candidatePoolDto.EntityStatus.ExternallyMastered ? 1 : 0);
                //TODO
                //SetUserGroupPeriod(groupDto, groupEntity);
                groupEntity.UserGroupTypeId = groupEntity.UserGroupTypeId == 0 ? (int)GrouptypeEnum.Default : groupEntity.UserGroupTypeId;
                groupEntity.ReferrerArchetypeId = candidatePoolDto.ReferrerArchetypeId.HasValue ? candidatePoolDto.ReferrerArchetypeId : groupEntity.ReferrerArchetypeId;
                groupEntity.ReferrerToken = candidatePoolDto.ReferrerToken ?? groupEntity.ReferrerToken;
                groupEntity.ReferrerResource = candidatePoolDto.ReferrerResource ?? groupEntity.ReferrerResource;
                return groupEntity;
            }


        }

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

        //private void GetUserGroupPeriod(UserGroupDtoBase groupDto, UserGroupEntity userGroup)
        //{
        //    if (!userGroup.PeriodId.HasValue)
        //        return;
        //    var period = _readonlyDataMemoryCache.Periods.FirstOrDefault(x => x.PeriodId == userGroup.PeriodId);
        //    if (period != null)
        //    {
        //    groupDto.Period = new PeriodDto
        //    {
        //        StartDate = period.Startdate,
        //        EndDate = period.Enddate,
        //        Name = period.ExtID
        //    };
        //    }
        //    else
        //    {
        //        groupDto.Period = null;
        //    }
            

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
