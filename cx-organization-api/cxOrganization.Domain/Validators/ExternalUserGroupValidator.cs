using System;
using System.Linq.Expressions;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class ExternalUserGroupValidator : UserGroupValidator
    {
        private readonly IHierarchyDepartmentService _hierachyDepartmentService;

        public ExternalUserGroupValidator(IHierarchyDepartmentService hierachyDepartmentService, 
            IOwnerRepository ownerRepository,
            IWorkContext workContext,
            IUserGroupRepository userGroupRepository) : base(ownerRepository, workContext, userGroupRepository)
        {
            _hierachyDepartmentService = hierachyDepartmentService;
        }

        public override UserGroupEntity Validate(ConexusBaseDto dto)
        {
            //Archetype must be ExternalUserGroup
            if (dto.Identity.Archetype != ArchetypeEnum.ExternalUserGroup)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            var groupDto = (ExternalUserGroupDto)dto;

            var department = _hierachyDepartmentService.GetH_DByDepartmentID(groupDto.ParentDepartmentId.Value, allowGetDepartmentDeleted: true, includeDepartment: true);
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
                if (userGroupEntity.UserId == null && userGroupEntity.DepartmentId != groupDto.ParentDepartmentId)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_CANDIDATE_POOL_NOT_BELONG_COMPANY);
                }

                if ( userGroupEntity.ArchetypeId != (int)ArchetypeEnum.ExternalUserGroup)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
                }
            }

            return userGroupEntity;
        }

        public override UserGroupEntity Validate(int userGroupId)
        {
            var entity = base.Validate(userGroupId);
            if (entity.ArchetypeId != (short)ArchetypeEnum.ExternalUserGroup)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return entity;
        }

        public override void ValidateMember(MemberDto member)
        {
            base.ValidateMember(member);
        }

        public override UserEntity ValidateMemberDto(int userGroupId, MemberDto member, ref UserGroupEntity userGroupEntity)
        {
            return base.ValidateMemberDto(userGroupId, member, ref userGroupEntity);
        }

        public override UserGroupEntity ValidateMembership(MemberDto memberDto)
        {
            return base.ValidateMembership(memberDto);
        }

        public override UserGroupEntity ValidateUserGroupDto(int userGroupId)
        {
            return base.ValidateUserGroupDto(userGroupId);
        }
    }
}
