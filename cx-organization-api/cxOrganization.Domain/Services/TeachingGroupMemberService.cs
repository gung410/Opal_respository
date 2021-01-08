using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Services
{
    public class TeachingGroupMemberService : ITeachingGroupMemberService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IUserGroupValidator _learnerUserGroupValidator;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserMappingService _userMappingService;
        private readonly IDepartmentValidator _departmentValidator;
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private readonly IUserGroupService _userGroupService;
        private readonly IWorkContext _workContext;

        public TeachingGroupMemberService(IUserGroupRepository userGroupRepository,
            OrganizationDbContext organizationDbContext,
            TeachingGroupValidator userGroupValidator,
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            IUserMappingService userMappingService,
            IDepartmentValidator departmentValidator,
            IDepartmentTypeRepository departmentTypeRepository,
            IWorkContext workContext,
            IUserGroupService userGroupService)
        {
            _userGroupRepository = userGroupRepository;
            _organizationDbContext = organizationDbContext;
            _learnerUserGroupValidator = userGroupValidator;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _userMappingService = userMappingService;
            _departmentValidator = departmentValidator;
            _departmentTypeRepository = departmentTypeRepository;
            _workContext = workContext;
            _userGroupService = userGroupService;
        }
        public MemberDto AddMember(HierarchyDepartmentValidationSpecification validationSpecification, int teachingGroupId, MemberDto memberDto)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            _learnerUserGroupValidator.ValidateMember(memberDto);

            var user = _userRepository.GetById(memberDto.Identity.Id);
            if (user == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_USER_NOT_FOUND);
            }
            if (user.ArchetypeId.HasValue && user.ArchetypeId.Value != (int)ArchetypeEnum.Learner)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Active)
            {
                _userGroupRepository.UpdateUG_U(teachingGroupId.ToString(), memberDto.Identity.Id.ToString(), new List<int>());
                memberDto = _userMappingService.ToMemberDto(user);
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Active;
            }
            else if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Inactive || memberDto.EntityStatus.StatusId == EntityStatusEnum.Deactive)
            {
                _userGroupRepository.UpdateUG_U(string.Empty, memberDto.Identity.Id.ToString(), new List<int>() { teachingGroupId });
                memberDto = _userMappingService.ToMemberDto(user);
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Inactive;
            }
            else
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);
            }
            _organizationDbContext.SaveChanges();
            return memberDto;
        }
        public List<MemberDto> GetMembers(HierarchyDepartmentValidationSpecification validationSpecification, int teachingGroupId)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            var userGroup = _userGroupRepository.GetUserGroupById(teachingGroupId, true, true, EntityStatusEnum.All);
            var results = new List<MemberDto>();
            var currentDate = new DateTime();
            var members = userGroup.UGMembers.Where(t => (t.validFrom == null || t.validFrom <= currentDate)
                                                         && (t.ValidTo == null || t.ValidTo >= currentDate));
            foreach (var uGU in members)
            {
                var item = uGU.User;
                if (item.ArchetypeId.HasValue && item.ArchetypeId.Value == (int)ArchetypeEnum.Learner)
                {
                    var memberDto = new MemberDto
                    {
                        Identity = new IdentityDto
                        {
                            Id = item.UserId,
                            Archetype = ArchetypeEnum.Learner,
                            CustomerId = item.CustomerId ?? 0,
                            ExtId = item.ExtId,
                            OwnerId = item.OwnerId
                        },
                        EntityStatus = new EntityStatusDto
                        {
                            EntityVersion = item.EntityVersion,
                            LastUpdated = item.LastUpdated,
                            LastUpdatedBy = item.LastUpdatedBy ?? 0,
                            StatusId = (EntityStatusEnum)item.EntityStatusId,
                            StatusReasonId = item.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)item.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                            LastExternallySynchronized = item.LastSynchronized,
                            ExternallyMastered = item.Locked == 1,
                            Deleted = item.Deleted.HasValue
                        },
                        Role = string.Empty
                    };
                    results.Add(memberDto);
                }
            }
            return results;
        }

        public MemberDto GetMember(HierarchyDepartmentValidationSpecification validationSpecification, int teachingGroupId, int memberId)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            var userGroup = _userGroupRepository.GetUserGroupById(teachingGroupId, true, true, EntityStatusEnum.All);
            var member = userGroup.UGMembers.FirstOrDefault(t => (t.validFrom == null && t.ValidTo == null) && t.UserId == memberId);
            var user = member != null ? member.User : null;
            if (user != null && (user.ArchetypeId.HasValue && user.ArchetypeId.Value == (int)ArchetypeEnum.Learner))
            {
                return new MemberDto
                {
                    Identity = new IdentityDto
                    {
                        Id = user.UserId,
                        Archetype = ArchetypeEnum.Learner,
                        CustomerId = user.CustomerId ?? 0,
                        ExtId = user.ExtId,
                        OwnerId = user.OwnerId
                    },
                    EntityStatus = new EntityStatusDto
                    {
                        EntityVersion = user.EntityVersion,
                        LastUpdated = user.LastUpdated,
                        LastUpdatedBy = user.LastUpdatedBy ?? 0,
                        StatusId = (EntityStatusEnum)user.EntityStatusId,
                        StatusReasonId = user.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)user.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                        LastExternallySynchronized = user.LastSynchronized,
                        ExternallyMastered = user.Locked == 1,
                        Deleted = user.Deleted.HasValue
                    },
                    Role = string.Empty
                };
            }
            else
                return null;
        }

        public List<MemberDto> GetUserGroupMemberShip(HierarchyDepartmentValidationSpecification validationSpecification, int userGroupId)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            var result = new List<MemberDto>();
            var userGroupEntity = _userGroupRepository.GetUserGroupIncludeDepartmentType(userGroupId, EntityStatusEnum.All);
            if (userGroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }

            foreach (var dtUg in userGroupEntity.DT_UGs)
            {
                var item = dtUg.DepartmentType;
                if (item != null && item.ArchetypeId == (int)ArchetypeEnum.Level)
                {
                    result.Add(new MemberDto
                    {
                        Identity = new IdentityDto
                        {
                            Id = item.DepartmentTypeId,
                            Archetype = item.ArchetypeId.HasValue ? (ArchetypeEnum)item.ArchetypeId : ArchetypeEnum.Unknown,
                            //CustomerId = item.CustomerId ?? item.CustomerId.Value,
                            ExtId = item.ExtId,
                            OwnerId = item.OwnerId
                        },
                        EntityStatus = new EntityStatusDto
                        {
                            //EntityVersion = item.EntityVersion,
                            //LastUpdated = item.LastUpdated,
                            //LastUpdatedBy = item.LastUpdatedBy ?? item.LastUpdatedBy.Value,
                            //StatusId = (StatusEnum)item.EntityStatusId,
                            //StatusReasonId = (StatusReasonEnum)item.EntityStatusReasonId,
                            //ExternallySynchronized = item.LastSynchronized
                        },
                        Role = string.Empty
                    });
                }
            }

            var departmentEntity = _departmentRepository.GetById(userGroupEntity.DepartmentId);
            if (departmentEntity != null && (departmentEntity.ArchetypeId.HasValue && departmentEntity.ArchetypeId.Value == (int)ArchetypeEnum.School))
            {
                result.Add(new MemberDto
                {
                    Identity = new IdentityDto
                    {
                        Id = departmentEntity.DepartmentId,
                        Archetype = ArchetypeEnum.School,
                        CustomerId = departmentEntity.CustomerId ?? 0,
                        ExtId = departmentEntity.ExtId,
                        OwnerId = departmentEntity.OwnerId
                    },
                    EntityStatus = new EntityStatusDto
                    {
                        EntityVersion = departmentEntity.EntityVersion,
                        LastUpdated = departmentEntity.LastUpdated,
                        LastUpdatedBy = departmentEntity.LastUpdatedBy ?? 0,
                        StatusId = (EntityStatusEnum)departmentEntity.EntityStatusId,
                        StatusReasonId = departmentEntity.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)departmentEntity.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                        LastExternallySynchronized = departmentEntity.LastSynchronized,
                        ExternallyMastered = departmentEntity.Locked == 1,
                        Deleted = departmentEntity.Deleted.HasValue
                    },
                    Role = string.Empty
                });
            }
            return result;
        }

        protected EntityStatusEnum ToEntityStatusEnum(short status, int? entityStatusId)
        {
            var entityStatus = EntityStatusEnum.Unknown;
            if (entityStatusId.HasValue)
            {
                entityStatus = (EntityStatusEnum)entityStatusId;
            }
            //Return Active
            if (status == 0 && (entityStatus == EntityStatusEnum.Active || entityStatus == EntityStatusEnum.Unknown))
            {
                return EntityStatusEnum.Active;
            }

            //Return Inactive
            if (status == 1 || entityStatus == EntityStatusEnum.Inactive)
            {
                return EntityStatusEnum.Inactive;
            }

            return EntityStatusEnum.Deactive;
        }

        public List<MemberDto> GetTeachingGroupMemberships(int teachingGroupId)
        {
            var result = new List<MemberDto>();
            var teachingGroupEntity = _userGroupRepository.GetUserGroupIncludeDepartmentType(teachingGroupId, EntityStatusEnum.All);
            if (teachingGroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }
            foreach (var dtUg in teachingGroupEntity.DT_UGs)
            {
                var item = dtUg.DepartmentType;
                if (item != null && item.ArchetypeId == (int)ArchetypeEnum.Level)
                {
                    result.Add(new MemberDto
                    {
                        Identity = new IdentityDto
                        {
                            Id = item.DepartmentTypeId,
                            Archetype = item.ArchetypeId.HasValue ? (ArchetypeEnum)item.ArchetypeId : ArchetypeEnum.Unknown,
                            ExtId = item.ExtId,
                            OwnerId = item.OwnerId
                        },
                        EntityStatus = new EntityStatusDto(),
                        Role = string.Empty
                    });
                }
            }
            return result;
        }


        public MemberDto UpdateTeachingGroupLevel(int teachingGroupId, MemberDto levelDto)
        {
            DepartmentTypeEntity teachingGroupLevelEntity = new DepartmentTypeEntity();
            UserGroupEntity teachingGroupEntity = InitParamForInsertOrRemoveDT(teachingGroupId, ref teachingGroupLevelEntity, levelDto);
            if (teachingGroupEntity.DT_UGs.Any(t => t.DepartmentType != null && t.DepartmentType.ExtId == teachingGroupLevelEntity.ExtId))
                return levelDto;
            _userGroupService.UpdateUserGroupDepartmentType(teachingGroupEntity, teachingGroupLevelEntity, true);
            return levelDto;
        }
        public MemberDto RemoveTeachingGroupLevel(int teachingGroupId, MemberDto levelDto)
        {
            DepartmentTypeEntity teachingGroupLevelEntity = new DepartmentTypeEntity();
            UserGroupEntity teachingGroupEntity = InitParamForInsertOrRemoveDT(teachingGroupId, ref teachingGroupLevelEntity, levelDto);
            if (!teachingGroupEntity.DT_UGs.Any(t => t.DepartmentType != null && t.DepartmentType.ExtId == teachingGroupLevelEntity.ExtId))
                return levelDto;

            var dtUg = teachingGroupEntity.DT_UGs.FirstOrDefault(t => t.DepartmentType != null && t.DepartmentType.ExtId == teachingGroupLevelEntity.ExtId);
            teachingGroupEntity.DT_UGs
                .Remove(dtUg);
            _organizationDbContext.SaveChanges();
            return levelDto;
        }
        private UserGroupEntity InitParamForInsertOrRemoveDT(int teachingGroupId, ref DepartmentTypeEntity teachingGroupLevelEntity, MemberDto memberDto)
        {
            if (memberDto.Identity.Archetype != ArchetypeEnum.Level)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }
            teachingGroupLevelEntity = _departmentTypeRepository.GetDepartmentTypeByExtId(memberDto.Identity.ExtId);
            if (teachingGroupLevelEntity.ArchetypeId != (int)ArchetypeEnum.Level)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            if (memberDto.Identity.CustomerId != _workContext.CurrentCustomerId || memberDto.Identity.OwnerId != _workContext.CurrentOwnerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_AND_OWNER_ID_NOTMATCH_WITH_CXTOKEN);
            }
            if (teachingGroupLevelEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.DEPARTMENT_TYPE_NOT_FOUND);
            }
            var includeProperties = QueryExtension.CreateIncludeProperties<UserGroupEntity>(x => x.DT_UGs);
            return _learnerUserGroupValidator.ValidateUserGroupDto(teachingGroupId);

        }
    }
}
