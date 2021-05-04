using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Newtonsoft.Json;

namespace cxOrganization.Domain.Mappings
{
    public class ApprovalGroupMappingService : IUserGroupMappingService
    {
        private readonly IDepartmentService _departmentService;
        private readonly IUserRepository _userRepository;

        private readonly IAdvancedWorkContext _workContext;
        public ApprovalGroupMappingService(
            IDepartmentService departmentService,
            IAdvancedWorkContext workContext,
            IPropertyService propertyService,
            IUserRepository userRepository)
        {
            _departmentService = departmentService;
            _workContext = workContext;
            _userRepository = userRepository;
        }
        public ConexusBaseDto ToUserGroupDto(UserGroupEntity groupEntity, bool? getDynamicProperties = null)
        {
            if (groupEntity == null || (groupEntity.ArchetypeId != (short)ArchetypeEnum.ApprovalGroup) || !groupEntity.UserGroupTypeId.HasValue)
            {
                return null;
            }

            var department = groupEntity.Department ??
                             (groupEntity.DepartmentId > 0
                                 ? _departmentService.GetDepartmentById(groupEntity.DepartmentId.Value)
                                 : null);
            var user = groupEntity.User ??
                       (groupEntity.UserId > 0 ? _userRepository.GetById(groupEntity.UserId) : null);
            var approvalGroupDto = new ApprovalGroupDto
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
                DepartmentId = groupEntity.DepartmentId,
                ApproverId = groupEntity.UserId,
                ReferrerArchetypeId = groupEntity.ReferrerArchetypeId,
                ReferrerToken = groupEntity.ReferrerToken,
                ReferrerResource = groupEntity.ReferrerResource
            };
            if (department != null)
            {
                approvalGroupDto.DepartmentName = department.Name;
                approvalGroupDto.Identity.CustomerId = department.CustomerId??0;
            }

            if (user != null)
            {
                if (approvalGroupDto.Identity.CustomerId == 0)
                    approvalGroupDto.Identity.CustomerId = user.CustomerId ?? 0;
                approvalGroupDto.Name = $"{user.FirstName} {user.LastName}".Trim();
                approvalGroupDto.EmailAddress = user.Email;

                if (!string.IsNullOrEmpty(user.DynamicAttributes))
                {
                    var jsonDynamic = JsonConvert.DeserializeObject<dynamic>(user.DynamicAttributes);
                    if (jsonDynamic != null)
                    {
                        string avatarUrl = Convert.ToString(jsonDynamic.avatarUrl ?? jsonDynamic.AvatarUrl);
                        approvalGroupDto.AvatarUrl = avatarUrl;
                    }
                }

            }


            return approvalGroupDto;
        }

        public UserGroupEntity ToUserGroupEntity(UserGroupEntity groupEntity, UserGroupDtoBase groupDto, IAdvancedWorkContext workContext = null)
        {
            var approvalGroup = groupDto as ApprovalGroupDto;

            approvalGroup.EntityStatus.LastUpdatedBy = approvalGroup.EntityStatus.LastUpdatedBy > 0
                                                             ? approvalGroup.EntityStatus.LastUpdatedBy
                                                             : workContext is object ? workContext.CurrentUserId : _workContext.CurrentUserId;

            if (groupDto.Identity.Id == 0)
            {
                var userGroup = new UserGroupEntity
                {
                    ArchetypeId = (int)groupDto.Identity.Archetype,
                    Name = approvalGroup.Name ?? string.Empty,
                    UserGroupTypeId = (int)approvalGroup.Type,
                    UserId = approvalGroup.ApproverId,
                    Description = approvalGroup.Description ?? string.Empty,
                    OwnerId = approvalGroup.Identity.OwnerId,
                    DepartmentId = approvalGroup.DepartmentId,
                    ExtId = approvalGroup.Identity.ExtId ?? string.Empty,
                    EntityStatusId = approvalGroup.EntityStatus.StatusId == EntityStatusEnum.Unknown ? (int)EntityStatusEnum.Inactive : (int)approvalGroup.EntityStatus.StatusId,
                    EntityStatusReasonId = (int)approvalGroup.EntityStatus.StatusReasonId,
                    LastUpdatedBy = approvalGroup.EntityStatus.LastUpdatedBy,
                    Created = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    ReferrerArchetypeId = approvalGroup.ReferrerArchetypeId,
                    ReferrerToken = approvalGroup.ReferrerToken ?? string.Empty,
                    ReferrerResource = approvalGroup.ReferrerResource ?? string.Empty
                };
                if (approvalGroup.EntityStatus.LastExternallySynchronized.HasValue)
                {
                    userGroup.LastSynchronized = approvalGroup.EntityStatus.LastExternallySynchronized.Value;
                }
                return userGroup;
            }
            else
            {
                if (groupEntity == null)
                    return null;
                CheckEntityVersion(approvalGroup.EntityStatus.EntityVersion, groupEntity.EntityVersion);
                groupEntity.Name = approvalGroup.Name ?? groupEntity.Name;
                if (groupEntity.UserGroupTypeId.HasValue)
                    groupEntity.UserGroupTypeId = (int)groupDto.Type;
                groupEntity.OwnerId = approvalGroup.Identity.OwnerId;
                groupEntity.DepartmentId = approvalGroup.DepartmentId;
                groupEntity.UserId = approvalGroup.ApproverId;
                groupEntity.ExtId = approvalGroup.Identity.ExtId ?? groupEntity.ExtId;
                groupEntity.Description = approvalGroup.Description ?? groupEntity.Description;
                groupEntity.LastUpdated = DateTime.Now;
                groupEntity.EntityStatusId = (short)approvalGroup.EntityStatus.StatusId;
                groupEntity.EntityStatusReasonId = (int)approvalGroup.EntityStatus.StatusReasonId;
                groupEntity.LastUpdatedBy = approvalGroup.EntityStatus.LastUpdatedBy;
                if (approvalGroup.EntityStatus.LastExternallySynchronized.HasValue)
                {
                    groupEntity.LastSynchronized = approvalGroup.EntityStatus.LastExternallySynchronized.Value;
                }
                //userGroup.Locked = (short)(approvalGroup.EntityStatus.ExternallyMastered ? 1 : 0);
                groupEntity.UserGroupTypeId = groupEntity.UserGroupTypeId == 0 ? (int)GrouptypeEnum.Default : groupEntity.UserGroupTypeId;
                groupEntity.ReferrerArchetypeId = approvalGroup.ReferrerArchetypeId.HasValue ? approvalGroup.ReferrerArchetypeId : groupEntity.ReferrerArchetypeId;
                groupEntity.ReferrerToken = approvalGroup.ReferrerToken ?? groupEntity.ReferrerToken;
                groupEntity.ReferrerResource = approvalGroup.ReferrerResource ?? groupEntity.ReferrerResource;
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
