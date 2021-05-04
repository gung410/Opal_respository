using cxOrganization.Client;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Domain.Validators
{
    public class UserPoolValidator : UserGroupValidator, IUserPoolValidator
    {
        private readonly IHierarchyDepartmentService _hierachyDepartmentService;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IAdvancedWorkContext _workContext;

        public UserPoolValidator(IHierarchyDepartmentService hierachyDepartmentService,
            IOwnerRepository ownerRepository,
            IUserGroupRepository userGroupRepository,
            IAdvancedWorkContext workContext)
            : base(ownerRepository, workContext, userGroupRepository)
        {
            _hierachyDepartmentService = hierachyDepartmentService;
            _userGroupRepository = userGroupRepository;
            _workContext = workContext;
        }

        public override UserGroupEntity Validate(ConexusBaseDto dto, IAdvancedWorkContext workContext = null)
        {
            // Archetype must be UserPool
            if (dto.Identity.Archetype != ArchetypeEnum.UserPool)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            var groupDto = (UserPoolDto)dto;

            var department = _hierachyDepartmentService.GetH_DByDepartmentID(groupDto.DepartmentId.Value, allowGetDepartmentDeleted: true, includeDepartment: true);
            if (department == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_DEPARTMENT_NOT_FOUND);
            }

            if (groupDto.Identity.CustomerId != department.Department.CustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_IS_NOT_CORRECT);
            }

            UserGroupEntity userGroupEntity = base.Validate(dto);

            // Take user entity from existing previous query
            if (userGroupEntity != null)
            {
                if (userGroupEntity.UserId == null && userGroupEntity.DepartmentId != groupDto.DepartmentId)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_USERGROUP_PROPERTY_VALIDATION,
                        "Department is not match with original data");
                }

                if (userGroupEntity.ArchetypeId != (int)ArchetypeEnum.UserPool)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
                }
            }

            return userGroupEntity;
        }

        public override UserGroupEntity ValidateMembership(MemberDto memberDto)
        {
            var usergroup = base.ValidateMembership(memberDto);

            if (usergroup.ArchetypeId != (short)ArchetypeEnum.UserPool)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            return usergroup;
        }

        public UserGroupEntity ValidateMembership(MembershipDto membershipDto)
        {
            if (membershipDto.Identity.CustomerId != _workContext.CurrentCustomerId || membershipDto.Identity.OwnerId != _workContext.CurrentOwnerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_AND_OWNER_ID_NOTMATCH_WITH_CXTOKEN);
            }
            if (membershipDto.GroupId <= 0)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_GROUP_ID_IS_REQUIRED);
            }
            var includeProperties = QueryExtension.CreateIncludeProperties<UserGroupEntity>(x => x.Department, x => x.User);

            var usergroupEntity = _userGroupRepository.GetUserGroupByIdsIncludeProperties(
                userGroupIds: new List<int> { membershipDto.GroupId },
                includeProperties: includeProperties,
                filters: null).FirstOrDefault();

            if (usergroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_USER_POOL_NOT_FOUND);
            }
            else
            {
                if (usergroupEntity.Department.CustomerId != membershipDto.Identity.CustomerId
                    || usergroupEntity.Department.OwnerId != membershipDto.Identity.OwnerId)
                    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_USERGROUP);

                if (usergroupEntity.ArchetypeId != (short)ArchetypeEnum.UserPool)
                    throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            return usergroupEntity;
        }

        public UserGroupEntity ValidateMemberships(int userPoolId, List<MembershipDto> membershipDtos)
        {
            ValidateInput(userPoolId, membershipDtos);

            var userGroupEntities = new List<UserGroupEntity>();
            
            var includeProperties = QueryExtension.CreateIncludeProperties<UserGroupEntity>(x => x.Department, x => x.User);

            var usergroupEntity = _userGroupRepository.GetUserGroupByIdsIncludeProperties(
                userGroupIds: new List<int> { membershipDtos[0].GroupId },
                includeProperties: includeProperties,
                filters: null).FirstOrDefault();

            if (usergroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_USER_POOL_NOT_FOUND);
            }
            else
            {
                if (usergroupEntity.ArchetypeId != (short)ArchetypeEnum.UserPool)
                    throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
                if (usergroupEntity.Department.CustomerId != membershipDtos[0].Identity.CustomerId
                    || usergroupEntity.Department.OwnerId != membershipDtos[0].Identity.OwnerId)    // Since all the identity have the same CXTOKEN so we just need to check the first one.
                    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_USERGROUP);
            }

            return usergroupEntity;
        }

        /// <summary>
        /// Validate the input to ensure that all the membershipDtos must:
        /// 1. ... have the same groupId.
        /// 2. ... have the memberId (which is the userId).
        /// 3. ... have the same CXTOKEN as the context in the Identity field.
        /// </summary>
        /// <param name="userPoolId">The user pool identifier</param>
        /// <param name="membershipDtos">The list of membershipDtos</param>
        private void ValidateInput(int userPoolId, List<MembershipDto> membershipDtos)
        {
            if (membershipDtos == null || !membershipDtos.Any())
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                    $"membershipDtos is required");

            if (!membershipDtos.All(m => m.GroupId > 0 && m.GroupId == userPoolId)
                || !membershipDtos.All(m => m.MemberId > 0))
            {
                for (int index = 0; index < membershipDtos.Count; index++)
                {
                    var membershipDto = membershipDtos[index];
                    if (membershipDto.GroupId <= 0)
                        throw new CXValidationException(cxExceptionCodes.ERROR_GROUP_ID_IS_REQUIRED,
                            $"membershipDtos[{index}].GroupId is required");

                    if (membershipDto.GroupId != userPoolId)
                        throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                            $"membershipDtos[{index}].GroupId '{membershipDtos[index].GroupId}' doesn't match with the userPoolId '{userPoolId}'");

                    if (membershipDto.MemberId == null || membershipDto.MemberId <= 0)
                        throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                            $"membershipDtos[{index}].MemberId is required");

                    if (membershipDto.Identity == null)
                        throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                            $"membershipDtos[{index}].Identity is required");

                    if (membershipDto.Identity.CustomerId != _workContext.CurrentCustomerId || membershipDto.Identity.OwnerId != _workContext.CurrentOwnerId)
                        throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_AND_OWNER_ID_NOTMATCH_WITH_CXTOKEN,
                            $"CustomerId or OwnerId of membershipDtos[{index}].Identity doesn't match with the CXTOKEN");
                }
            }
        }
    }
}
