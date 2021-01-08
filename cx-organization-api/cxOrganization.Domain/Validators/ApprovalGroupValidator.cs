using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class ApprovalGroupValidator : UserGroupValidator
    {
        private readonly IHierarchyDepartmentService _hierachyDepartmentService;
        private readonly IUserGroupRepository _userGroupRepository;

        public ApprovalGroupValidator(IHierarchyDepartmentService hierachyDepartmentService,
            IOwnerRepository ownerRepository,
            IUserGroupRepository userGroupRepository,
            IWorkContext workContext)
            : base(ownerRepository, workContext, userGroupRepository)
        {
            _hierachyDepartmentService = hierachyDepartmentService;
            _userGroupRepository = userGroupRepository;
        }

        public override UserGroupEntity Validate(ConexusBaseDto dto)
        {
            //Archetype must be ApprovalGroup
            if (dto.Identity.Archetype != ArchetypeEnum.ApprovalGroup)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            var groupDto = (ApprovalGroupDto)dto;

            if (groupDto.Type != GrouptypeEnum.AlternativeApprovalGroup && groupDto.Type != GrouptypeEnum.PrimaryApprovalGroup)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_USERGROUP_PROPERTY_VALIDATION, $"GroupType is not allowed. Must be [{GrouptypeEnum.PrimaryApprovalGroup}, " +
                    $"{GrouptypeEnum.AlternativeApprovalGroup}]", cxStudioExceptionType.BadRequest);
            }

            var department = _hierachyDepartmentService.GetH_DByDepartmentID(groupDto.DepartmentId.Value, allowGetDepartmentDeleted: true, includeDepartment: true);
            if (department == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }

            if (groupDto.Identity.CustomerId != department.Department.CustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_IS_NOT_CORRECT);
            }

            if (groupDto.Identity.Id == 0)
            {
                var usergroupBelongs = _userGroupRepository.GetUserGroupsWithoutPaging(userGroupIds: new List<int> { (int)groupDto.Type }, 
                                                                                       parentUserIds: groupDto.ApproverId.HasValue ? new List<int> { groupDto.ApproverId.Value } : null,
                                                                                       userGroupStatuses: new List<EntityStatusEnum> { EntityStatusEnum.Active });
                if (usergroupBelongs.Any())
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_USERGROUP_PROPERTY_VALIDATION, $"Cannot have more than one {groupDto.Type}", cxStudioExceptionType.Conflict);
                }
            }

            UserGroupEntity userGroupEntity = base.Validate(dto);

            //Take user entity from existing previous query
            if (userGroupEntity != null)
            {


                if (userGroupEntity.UserId == null && (userGroupEntity.DepartmentId != groupDto.DepartmentId))
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_USERGROUP_PROPERTY_VALIDATION,
                        "Department is not match with orginal data");
                }

                if (userGroupEntity.ArchetypeId != (int)ArchetypeEnum.ApprovalGroup)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
                }
            }

            return userGroupEntity;
        }
    }
}
