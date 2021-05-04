using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class UserGroupValidator : IUserGroupValidator
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IUserGroupRepository _userGroupRepository;

        public UserGroupValidator(IOwnerRepository ownerRepository,
            IAdvancedWorkContext workContext,
            IUserGroupRepository userGroupRepository)
        {
            _ownerRepository = ownerRepository;
            _workContext = workContext;
            _userGroupRepository = userGroupRepository;
        }

        public virtual UserGroupEntity Validate(ConexusBaseDto dto, IAdvancedWorkContext workContext = null)
        {
            var groupDto = (UserGroupDtoBase)dto;

            //Entity status must be set
            if (groupDto.EntityStatus.StatusId == EntityStatusEnum.Unknown)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUS_ID_REQUIRED);

            var owner = _ownerRepository.GetById(groupDto.Identity.OwnerId);
            if (owner == null)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_OWNERID_NOT_FOUND);
            }

            var currentCustomerId = workContext is object ? workContext.CurrentCustomerId : _workContext.CurrentCustomerId;

            if (groupDto.Identity.CustomerId != currentCustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_ID_NOT_MATCH);
            }

            var userGroupId = (int?)groupDto.Identity.Id;

            var userGroupEntities = _userGroupRepository.GetUserGroups(userGroupId, groupDto.Identity.ExtId, EntityStatusEnum.All);
            //Check duplicate ExtId
            if (!string.IsNullOrEmpty(groupDto.Identity.ExtId))
            {
                if (userGroupEntities.Any(p => p.UserGroupId != userGroupId
                                        && p.ExtId == groupDto.Identity.ExtId
                                        && p.ArchetypeId == (short)groupDto.Identity.Archetype
                                        && ((p.Department != null && p.Department.CustomerId == groupDto.Identity.CustomerId)
                                        || (p.User != null && p.User.CustomerId == groupDto.Identity.CustomerId))))
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_USERGROUP_EXTID_EXISTS_CUSTOMER);
                }
            }
            UserGroupEntity userGroupEntity = null;
            //Check user exists based on identify ID
            if (userGroupId > 0)
            {
                userGroupEntity = userGroupEntities.FirstOrDefault(p => p.UserGroupId == userGroupId);
                if (userGroupEntity == null || !userGroupEntity.UserGroupTypeId.HasValue)
                    throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("Id : {0}", userGroupId));
            }

            return userGroupEntity;
        }

        public virtual void ValidateMember(MemberDto member)
        {
            throw new NotImplementedException();
        }
        public virtual UserEntity ValidateMemberDto(int userGroupId, MemberDto member, ref UserGroupEntity userGroupEntity)
        {
            throw new NotImplementedException();
        }

        public virtual UserGroupEntity ValidateUserGroupDto(int userGroupId)
        {
            throw new NotImplementedException();
        }

        public virtual UserGroupEntity ValidateMembership(MemberDto memberDto)
        {
            if (memberDto.Identity.CustomerId != _workContext.CurrentCustomerId || memberDto.Identity.OwnerId != _workContext.CurrentOwnerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_AND_OWNER_ID_NOTMATCH_WITH_CXTOKEN);
            }
            if (!memberDto.Identity.Id.HasValue)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);
            }
            var includeProperties = QueryExtension.CreateIncludeProperties<UserGroupEntity>(x => x.Department, x => x.User);

            var usergroupEntity = _userGroupRepository.GetUserGroupByIdsIncludeProperties(userGroupIds: new List<int> { (int)memberDto.Identity.Id },
                includeProperties: includeProperties,
                filters: null).FirstOrDefault();

            if (usergroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }
            else
            {
                if (usergroupEntity.Department.CustomerId != memberDto.Identity.CustomerId
                    || usergroupEntity.Department.OwnerId != memberDto.Identity.OwnerId)
                    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_USERGROUP);
            }

            if (usergroupEntity.ArchetypeId != (short)memberDto.Identity.Archetype)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }

            return usergroupEntity;
        }

        public virtual UserGroupEntity Validate(int userGroupId)
        {
            if (userGroupId <= 0)
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);

            var userGroupEntity = _userGroupRepository.GetById(userGroupId);

            if (userGroupEntity == null)
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);

            return userGroupEntity;
        }
    }
}
