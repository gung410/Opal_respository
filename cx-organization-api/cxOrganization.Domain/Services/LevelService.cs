using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Services
{
    public class LevelService : ILevelService
    {
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IDepartmentMappingService _departmentMappingService;
        private readonly IUserMappingService _userMappingService;
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserGroupMappingService _userGroupMappingService;
        private readonly IUserGroupService _userGroupService;
        private readonly IDepartmentValidator _departmentvalidator;

        public LevelService(OrganizationDbContext organizationDbContext,
            IDepartmentRepository departmentRepository,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IAdvancedWorkContext workContext,
            IDepartmentMappingService departmentMappingService,
            IUserMappingService userMappingService,
            IDepartmentTypeRepository departmentTypeRepository,
            IUserTypeRepository userTypeRepository,
            IUserGroupRepository userGroupRepository,
            IUserGroupMappingService userGroupMappingService,
            IUserGroupService userGroupService,
            IDepartmentValidator departmentValidator)
        {
            _organizationDbContext = organizationDbContext;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _workContext = workContext;
            _departmentMappingService = departmentMappingService;
            _userMappingService = userMappingService;
            _departmentTypeRepository = departmentTypeRepository;
            _userTypeRepository = userTypeRepository;
            _userGroupRepository = userGroupRepository;
            _userGroupMappingService = userGroupMappingService;
            _userGroupService = userGroupService;
            _departmentvalidator = departmentValidator;
        }

        public MemberDto AddOrRemoveMember(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId, MemberDto memberDto)
        {
            var parenthd = _departmentvalidator.ValidateHierarchyDepartment(validationSpecification);

            if (memberDto.Identity.Archetype == ArchetypeEnum.Class
                || memberDto.Identity.Archetype == ArchetypeEnum.School
                || memberDto.Identity.Archetype == ArchetypeEnum.SchoolOwner)
            {
                memberDto = AddMemberAsDepartment(parenthd.DepartmentId, levelExtId, memberDto);
            }
            else if (memberDto.Identity.Archetype == ArchetypeEnum.Learner)
            {
                memberDto = AddMemberAsLearner(parenthd.DepartmentId, levelExtId, memberDto);
            }
            else if (memberDto.Identity.Archetype == ArchetypeEnum.TeachingGroup)
            {
                memberDto = AddMemberAsUserGroup(parenthd.DepartmentId, levelExtId, memberDto);
            }
            else
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            _organizationDbContext.SaveChanges();
            return memberDto;
        }

        private MemberDto AddMemberAsDepartment(int parentDepartmentId, string levelExtId, MemberDto memberDto)
        {
            var departmentType = _organizationDbContext.Set<DepartmentTypeEntity>().FirstOrDefault(p => p.ExtId == levelExtId);
            if (departmentType == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_NOT_FOUND);
            }

            if (!departmentType.ArchetypeId.HasValue || (departmentType.ArchetypeId.HasValue && departmentType.ArchetypeId.Value != (int)ArchetypeEnum.Level))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_INVALID);
            }

            if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Active)
            {
                if (!memberDto.Identity.Id.HasValue)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
                }
                var departmentId = (int)memberDto.Identity.Id.Value;
                var department = _departmentRepository.GetDepartmentIncludeDepartmentTypes(departmentId, _workContext.CurrentOwnerId);

                if (department == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
                }

                CheckADepartmentBelongToParent(parentDepartmentId, department.DepartmentId, memberDto);

                if (department.ArchetypeId != (int)memberDto.Identity.Archetype)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
                }
                AddRemoveDepartmentTypes(department, new List<int> { departmentType.DepartmentTypeId },
                    new List<int>());
                _departmentRepository.Update(department);
                memberDto = _departmentMappingService.ToMemberDto(department);
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Active;
            }
            else if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Inactive)
            {
                if (!memberDto.Identity.Id.HasValue)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
                }
                var departmentId = (int)memberDto.Identity.Id.Value;
                var department = _departmentRepository.GetDepartmentIncludeDepartmentTypes(departmentId, _workContext.CurrentOwnerId);
                if (department == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
                }

                CheckADepartmentBelongToParent(parentDepartmentId, department.DepartmentId, memberDto);

                if (department.ArchetypeId != (int)memberDto.Identity.Archetype)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
                }
                AddRemoveDepartmentTypes(department, new List<int>(),
                    new List<int> { departmentType.DepartmentTypeId });
                _departmentRepository.Update(department);
                memberDto = _departmentMappingService.ToMemberDto(department);
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Inactive;
            }
            else
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);
            }
            return memberDto;
        }
        private void AddRemoveDepartmentTypes(DepartmentEntity department, List<int> departmentTypeIdsToAdd, List<int> departmentTypeIdsToRemove)
        {
            if (department != null)
            {
                DTDEntity departmentTypeDepartment;
                if (departmentTypeIdsToRemove != null)
                {
                    foreach (int typeId in departmentTypeIdsToRemove)
                    {
                        departmentTypeDepartment = department.DT_Ds.FirstOrDefault(x => x.DepartmentTypeId == typeId);
                        if (departmentTypeDepartment != null) // Remove from user if exist!
                        {
                            department.DT_Ds.Remove(departmentTypeDepartment);
                        }
                    }
                }
                foreach (int typeId in departmentTypeIdsToAdd)
                {
                    departmentTypeDepartment = department.DT_Ds.FirstOrDefault(x => x.DepartmentTypeId == typeId);
                    if (departmentTypeDepartment == null) // Add to user if not exist!
                    {
                        var departmentType = _departmentTypeRepository.GetById(typeId);
                        if (departmentType != null)
                        {
                            department.DT_Ds.Add(new DTDEntity { DepartmentTypeId = departmentType.DepartmentTypeId, DepartmentId = department.DepartmentId });
                        }
                    }
                }
            }
        }

        public List<MemberDto> GetMemberships(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId)
        {

            var departmentType = _departmentTypeRepository.GetDepartmentTypeByExtId(levelExtId);
            if (departmentType == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_NOT_FOUND);
            }

            if (!departmentType.ArchetypeId.HasValue || (departmentType.ArchetypeId.HasValue && departmentType.ArchetypeId.Value != (int)ArchetypeEnum.Level))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_INVALID);
            }
            var parentHd = _departmentvalidator.ValidateHierarchyDepartment(validationSpecification);
            var result = new List<MemberDto>();
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hierarchyId = currentHD.HierarchyId;
            var schoolHd = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(hierarchyId, parentHd.DepartmentId);

            var childDepartments = _hierarchyDepartmentRepository.GetChildHds(schoolHd.Path, true, true, new List<int> { departmentType.DepartmentTypeId });

            // Add Classes Membership
            foreach (var item in childDepartments)
            {
                if (item.Department.ArchetypeId == (int)ArchetypeEnum.Class)
                {
                    result.Add(_departmentMappingService.ToMemberDto(item.Department));
                }
            }

            //Add Schools Membership
            var schoolEntity = _departmentRepository.GetById(parentHd.DepartmentId);
            if (schoolEntity != null && (!schoolEntity.ArchetypeId.HasValue || schoolEntity.ArchetypeId.Value == (int)ArchetypeEnum.School))
            {
                result.Add(_departmentMappingService.ToMemberDto(schoolEntity));
            }

            //Add Teaching Group Membership
            var userGroups = _userGroupService.GetUserGroupByDepartmentId(parentHd.DepartmentId, departmentTypeId: departmentType.DepartmentTypeId, filters: new[] { EntityStatusEnum.All });
            foreach (var userGroupEntity in userGroups)
            {
                if (userGroupEntity.ArchetypeId.HasValue)
                {
                    result.Add(_userGroupMappingService.ToMemberDto(userGroupEntity));
                }
            }
            return result;
        }

        private void CheckADepartmentBelongToParent(int parentDepartmentId, int departmentId, MemberDto memberDto)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hds = _hierarchyDepartmentRepository.GetListHierarchyDepartmentEntity(currentHD.HierarchyId, departmentId, parentDepartmentId);
            var departmentHd = hds.FirstOrDefault(p => p.DepartmentId == departmentId);
            var parentDepartmentHd = hds.FirstOrDefault(p => p.DepartmentId == parentDepartmentId);
            if (departmentHd == null || parentDepartmentHd == null || !departmentHd.Path.StartsWith(parentDepartmentHd.Path))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_BELONG_TO_PARENT_DEPARTMENT);
            }
        }

        private MemberDto AddMemberAsLearner(int parentDepartmentId, string levelExtId, MemberDto memberDto)
        {
            var usertype = _userTypeRepository.GetUserTypeByExtId(levelExtId);
            if (usertype == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_NOT_FOUND);
            }

            if (!usertype.ArchetypeId.HasValue || (usertype.ArchetypeId.HasValue && usertype.ArchetypeId.Value != (int)ArchetypeEnum.Level))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_INVALID);
            }

            if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Active)
            {
                if (!memberDto.Identity.Id.HasValue)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
                }
                var userId = (int)memberDto.Identity.Id.Value;
                var user = _userRepository.GetUserById(userId);

                if (user == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
                }

                CheckADepartmentBelongToParent(parentDepartmentId, user.DepartmentId, memberDto);

                if (user.ArchetypeId != (int)memberDto.Identity.Archetype)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
                }
                _userRepository.AddRemoveUserTypes(user, new List<int> { usertype.UserTypeId }, new List<int>());
                _userRepository.Update(user);
                memberDto = _userMappingService.ToMemberDto(user);
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Active;
            }
            else if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Inactive)
            {
                if (!memberDto.Identity.Id.HasValue)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
                }
                var userId = (int)memberDto.Identity.Id.Value;
                var user = _userRepository.GetUserById(userId);
                if (user == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
                }

                CheckADepartmentBelongToParent(parentDepartmentId, user.DepartmentId, memberDto);

                if (user.ArchetypeId != (int)memberDto.Identity.Archetype)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
                }
                _userRepository.AddRemoveUserTypes(user, new List<int>(), new List<int> { usertype.UserTypeId });
                _userRepository.Update(user);
                memberDto = _userMappingService.ToMemberDto(user);
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Inactive;
            }
            else
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);
            }
            return memberDto;
        }

        private MemberDto AddMemberAsUserGroup(int parentDepartmentId, string levelExtId, MemberDto memberDto)
        {
            var departmentType = _departmentTypeRepository.GetDepartmentTypeByExtId(levelExtId);
            if (departmentType == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_NOT_FOUND);
            }

            if (!departmentType.ArchetypeId.HasValue || (departmentType.ArchetypeId.HasValue && departmentType.ArchetypeId.Value != (int)ArchetypeEnum.Level))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_INVALID);
            }

            if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Active)
            {
                if (!memberDto.Identity.Id.HasValue)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
                }
                var userGroupId = (int)memberDto.Identity.Id.Value;
                var userGroupEntity = _userGroupRepository.GetUserGroupIncludeDepartmentType(userGroupId, EntityStatusEnum.All);
                if (userGroupEntity == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
                }

                CheckADepartmentBelongToParent(parentDepartmentId, userGroupEntity.DepartmentId.Value, memberDto);

                if (userGroupEntity.ArchetypeId != (int)memberDto.Identity.Archetype)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
                }
                userGroupEntity.DT_UGs.Add(new DTUGEntity { DepartmentTypeId = departmentType.DepartmentTypeId, UserGroupId = userGroupEntity.UserGroupId });
                _userGroupRepository.Update(userGroupEntity);
                memberDto = _userGroupMappingService.ToMemberDto(userGroupEntity);
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Active;
            }
            else if (memberDto.EntityStatus.StatusId == EntityStatusEnum.Inactive)
            {
                if (!memberDto.Identity.Id.HasValue)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
                }
                var userGroupId = (int)memberDto.Identity.Id.Value;
                var userGroup = _userGroupRepository.GetUserGroupIncludeDepartmentType(userGroupId, EntityStatusEnum.All);
                if (userGroup == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
                }
                var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                var departmentHd = _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, userGroup.DepartmentId.Value);
                var parentDepartmentHd = _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, parentDepartmentId);
                if (!departmentHd.Path.StartsWith(parentDepartmentHd.Path))
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_BELONG_TO_PARENT_DEPARTMENT);
                }
                if (userGroup.ArchetypeId != (int)memberDto.Identity.Archetype)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
                }
                var dtUg = userGroup.DT_UGs.FirstOrDefault(t => t.DepartmentTypeId == departmentType.DepartmentTypeId);
                if (dtUg != null)
                    userGroup.DT_UGs.Remove(dtUg);
                _userGroupRepository.Update(userGroup);
                memberDto = _userGroupMappingService.ToMemberDto(userGroup);
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Inactive;
            }
            else
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUSID_IS_INVALID);
            }
            return memberDto;
        }

        public MemberDto GetMemberAsDepartment(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId, int departmentId)
        {
            var parentHd = _departmentvalidator.ValidateHierarchyDepartment(validationSpecification);
            var departmentType = _departmentTypeRepository.GetDepartmentTypeByExtId(levelExtId);
            if (departmentType == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_NOT_FOUND);
            }

            if (!departmentType.ArchetypeId.HasValue || (departmentType.ArchetypeId.HasValue && departmentType.ArchetypeId.Value != (int)ArchetypeEnum.Level))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_INVALID);
            }
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var departmentHd = _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, departmentId);
            var parentDepartmentHd = _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, parentHd.DepartmentId);

            var department = _departmentRepository.GetDepartmentIncludeDepartmentTypes(departmentId);
            if (department != null && department.DT_Ds.Select(x => x.DepartmentTypeId).Contains(departmentType.DepartmentTypeId)
                && departmentHd.Path.StartsWith(parentDepartmentHd.Path))
            {
                var memberDto = _departmentMappingService.ToMemberDto(department);

                return memberDto;
            }
            else
                throw new CXValidationException(cxExceptionCodes.ERROR_DEPARTMENT_NOT_FOUND);
        }

        public MemberDto GetMemberAsTeachingGroup(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId, int teachingGroupId)
        {
            var parentHd = _departmentvalidator.ValidateHierarchyDepartment(validationSpecification);
            var departmentType = _departmentTypeRepository.GetDepartmentTypeByExtId(levelExtId);
            if (departmentType == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_NOT_FOUND);
            }

            if (!departmentType.ArchetypeId.HasValue || (departmentType.ArchetypeId.HasValue && departmentType.ArchetypeId.Value != (int)ArchetypeEnum.Level))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_INVALID);
            }
            var userGroup = _userGroupRepository.GetUserGroupIncludeDepartmentType(teachingGroupId, EntityStatusEnum.All);
            if (userGroup != null && userGroup.DT_UGs.Select(x => x.DepartmentTypeId).Contains(departmentType.DepartmentTypeId) && userGroup.DepartmentId.Value == parentHd.DepartmentId)
            {
                var memberDto = _userGroupMappingService.ToMemberDto(userGroup);


                return memberDto;
            }
            throw new CXValidationException(cxExceptionCodes.ERROR_TEACHINGGROUP_NOT_FOUND);
        }

        public MemberDto GetMemberAsUser(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId, int userId)
        {
            var parentHd = _departmentvalidator.ValidateHierarchyDepartment(validationSpecification);
            var usertype = _userTypeRepository.GetUserTypeByExtId(levelExtId);
            if (usertype == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_NOT_FOUND);
            }

            if (!usertype.ArchetypeId.HasValue || (usertype.ArchetypeId.HasValue && usertype.ArchetypeId.Value != (int)ArchetypeEnum.Level))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_INVALID);
            }
            var user = _userRepository.GetUserById(userId);
            if (user != null && user.UT_Us.Select(x => x.UserTypeId).Contains(usertype.UserTypeId))
            {
                var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                var departmentHd = _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, user.DepartmentId);
                if (departmentHd == null)
                    throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
                var parentDepartmentHd = _hierarchyDepartmentRepository.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, parentHd.DepartmentId);
                if (parentDepartmentHd == null)
                    throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
                if (!departmentHd.Path.StartsWith(parentDepartmentHd.Path))
                    throw new CXValidationException(cxExceptionCodes.ERROR_USER_NOT_FOUND);
                var memberDto = _userMappingService.ToMemberDto(user);

                return memberDto;
            }
            else
                throw new CXValidationException(cxExceptionCodes.ERROR_USER_NOT_FOUND);

        }

        public List<MemberDto> GetMembersAsUser(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId)
        {
            var parentHd = _departmentvalidator.ValidateHierarchyDepartment(validationSpecification);
            var usertype = _userTypeRepository.GetUserTypeByExtId(levelExtId);
            if (usertype == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_NOT_FOUND);
            }

            if (!usertype.ArchetypeId.HasValue || (usertype.ArchetypeId.HasValue && usertype.ArchetypeId.Value != (int)ArchetypeEnum.Level))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_LEVEL_INVALID);
            }

            var result = new List<MemberDto>();
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var departmentHd = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(currentHD.HierarchyId, parentHd.DepartmentId);
            var childHds = _hierarchyDepartmentRepository.GetChildHds(departmentHd.Path, includeInActiveStatus: true);
            var departmentIds = childHds.Select(x => x.DepartmentId).ToList();
            departmentIds.Insert(0, parentHd.DepartmentId);

            var users = _userRepository.GetUsersByDepartmentIdsAndUserTypeIds(departmentIds, new List<int> { usertype.UserTypeId });

            foreach (var item in users)
            {
                if ((item.ArchetypeId == (int)ArchetypeEnum.Learner || item.ArchetypeId == (int)ArchetypeEnum.Employee))
                {
                    result.Add(_userMappingService.ToMemberDto(item));
                }
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

            return entityStatus;
        }
    }
}
