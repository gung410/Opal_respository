using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class TeachingGroupValidator : UserGroupValidator
    {
        private readonly IHierarchyDepartmentService _hierachyDepartmentService;
        private readonly IUserGroupRepository _userGroupRepository;

        public TeachingGroupValidator(IHierarchyDepartmentService hierachyDepartmentService,
            IOwnerRepository ownerRepository,
            IUserGroupRepository userGroupRepository,
            IAdvancedWorkContext workContext)
            : base(ownerRepository, workContext, userGroupRepository)
        {
            _hierachyDepartmentService = hierachyDepartmentService;
            _userGroupRepository = userGroupRepository;
        }

        public override UserGroupEntity Validate(ConexusBaseDto dto, IAdvancedWorkContext workContext = null)
        {
            //Archetype must be TeachingGroup
            if (dto.Identity.Archetype != ArchetypeEnum.TeachingGroup && dto.Identity.Archetype != ArchetypeEnum.EducationProgram)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }
            
            var groupDto = (TeachingGroupDto)dto;

            var department = _hierachyDepartmentService.GetH_DByDepartmentID(groupDto.SchoolId.Value, allowGetDepartmentDeleted: true, includeDepartment: true);
            if (department == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }

            if (groupDto.Identity.CustomerId != department.Department.CustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_IS_NOT_CORRECT);
            }

            UserGroupEntity userGroupEntity = base.Validate(dto);

            //Take user entity from existing previous query
            if (userGroupEntity != null)
            {
                if (userGroupEntity.UserId == null && userGroupEntity.DepartmentId != groupDto.SchoolId)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_TEACHINGGROUP_NOT_BELONG_SCHOOL);
                }

                if (userGroupEntity.ArchetypeId != (int)ArchetypeEnum.TeachingGroup)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
                }
            }

            return userGroupEntity;
        }

        public override void ValidateMember(MemberDto member)
        {
            if (member.Identity.Archetype != ArchetypeEnum.Learner)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }
        }
        public override UserEntity ValidateMemberDto(int userGroupId, MemberDto member, ref UserGroupEntity userGroupEntity)
        {
            return null;
        }

        public override UserGroupEntity ValidateUserGroupDto(int teachingGroupId)
        {
            UserGroupEntity teachingGroupEntity = _userGroupRepository.GetUserGroupByIds(userGroupIds: new List<int>() { teachingGroupId },
                allowArchetypeIds: new List<int>() { (int)ArchetypeEnum.TeachingGroup },
                includeDepartmenttype: true).FirstOrDefault();

            if (teachingGroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_TEACHINGGROUP_NOT_FOUND);
            }

            if (teachingGroupEntity.ArchetypeId != (int)ArchetypeEnum.TeachingGroup)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return teachingGroupEntity;
        }
        public override UserGroupEntity ValidateMembership(MemberDto memberDto)
        {
            var usergroup = base.ValidateMembership(memberDto);

            if (usergroup.ArchetypeId != (short)ArchetypeEnum.TeachingGroup)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }

            return usergroup;
        }

        public override UserGroupEntity Validate(int userGroupId)
        {
            var entity = base.Validate(userGroupId);
            if (entity.ArchetypeId != (short)ArchetypeEnum.TeachingGroup)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return entity;
        }
    }
}
