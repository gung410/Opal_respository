using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Common;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class UserValidator : IUserValidator
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdvancedWorkContext _workContext;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;

        public UserValidator(IUserRepository userRepository,
            IAdvancedWorkContext workContext,
            ICustomerRepository customerRepository,
            IOwnerRepository ownerRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository)
        {
            _userRepository = userRepository;
            _workContext = workContext;
            _customerRepository = customerRepository;
            _ownerRepository = ownerRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
        }

        public virtual UserEntity Validate(DepartmentEntity parentDepartment,
            ConexusBaseDto dto,
            int? currentOwnerId = null,
            int? currentCustomerId = null)
        {
            _workContext.CurrentCustomerId = currentCustomerId ?? _workContext.CurrentCustomerId;
            _workContext.CurrentOwnerId = currentOwnerId ?? _workContext.CurrentOwnerId;

            var userDto = (UserDtoBase)dto;

            //Skip check EmailAddress when it's empty
            if (userDto.EmailAddress == string.Empty)
            {
                userDto.EmailAddress = null;
            }

            UserEntity userEntity = null;           
            //Check SSN
            if (!string.IsNullOrEmpty(userDto.SSN))
            {
                if (!Utilities.ValidateFormatSsn(userDto.SSN))
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_SSN_IS_INVALID);
                }
            }

            //Entity status must be set
            if (userDto.EntityStatus.StatusId == EntityStatusEnum.Unknown)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUS_ID_REQUIRED);

            }
            //If an object is "Locked" then the ExtId is mandatory to be set with a value
            if (userDto.EntityStatus.ExternallyMastered && string.IsNullOrEmpty(userDto.Identity.ExtId))
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_IDENTITY_LOCKED_EXTID_MANDATORY);
            }

            //Validate OwnerId
            if (userDto.Identity.OwnerId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_OWNERID_REQUIRED);
            }
            var owner = _ownerRepository.GetById(userDto.Identity.OwnerId);
            if (owner == null)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_OWNERID_NOT_FOUND);
            }
            
            //Validate CustomerId
            if (userDto.Identity.CustomerId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_REQUIRED);
            }
            if (userDto.Identity.CustomerId != _workContext.CurrentCustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_ID_NOT_MATCH);
            }
            var customer = _customerRepository.GetById(userDto.Identity.CustomerId);
            if (customer == null)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_NOT_FOUND);
            }
            var userId = (int?)userDto.Identity.Id;
            var userEntities = _userRepository.GetUsers(currentOwnerId ?? _workContext.CurrentOwnerId, userId, string.Empty, userDto.Identity.ExtId);
            //Check duplicate ExtId
            if (!string.IsNullOrEmpty(userDto.Identity.ExtId))
            {
                var usersFilterByExtId = userEntities.Where(p => p.ExtId == userDto.Identity.ExtId).ToList();
                if (usersFilterByExtId.Any(p =>
                (p.UserId != userDto.Identity.Id || userDto.Identity.Id == 0)
                        && p.CustomerId == userDto.Identity.CustomerId
                        && p.ArchetypeId == (short)userDto.Identity.Archetype
                        && !p.Deleted.HasValue))
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_USER_EXTID_EXISTS_CUSTOMER);
                }
            }
            //Check user exists based on identify ID
            if (userId > 0)
            {
                userEntity = userEntities.FirstOrDefault(p => p.UserId == userId);
                if(userEntity == null)
                    throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("Id : {0}", userId));
            }
            return userEntity;
        }

        public virtual void ValidateMember(MemberDto member)
        {
            throw new NotImplementedException();
        }

        public virtual HierarchyDepartmentEntity ValidateHierarchyDepartment(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var departmentIds = hierarchyDepartmentValidationDto.HierarchyDepartments.Select(p => p.Key).ToArray();
            List<HierarchyDepartmentEntity> hds = _hierarchyDepartmentRepository.GetListHierarchyDepartmentEntity(currentHD.HierarchyId, departmentIds);

            //In case we need to check the parent department is not a direct parent department
            //We need to get all HD from the leaf to the top, and check the child belong to the hierachy
            List<int> listDepartmentIdsFromTheCurrentToTheTop = new List<int>();
            if (!hierarchyDepartmentValidationDto.IsDirectParent)
            {
                if (hds.Count == 0)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
                }
                listDepartmentIdsFromTheCurrentToTheTop = _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(hds.Last().HDId, true);
            }
            
            HierarchyDepartmentEntity previousHierarchyDepartment = null;
            foreach (var hierarchyDepartment in hierarchyDepartmentValidationDto.HierarchyDepartments)
            {
                //Check department must exist
                var currentHierarchyDepartment = hds.FirstOrDefault(p => p.DepartmentId == hierarchyDepartment.Key);
                if (currentHierarchyDepartment == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
                }

                //Check matching archetype
                if (!hierarchyDepartmentValidationDto.SkipCheckingArchetype)
                {
                    var archetypeShouldMatch = hierarchyDepartment.Value;
                    if (currentHierarchyDepartment.Department.ArchetypeId != (int) archetypeShouldMatch)
                    {
                        throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
                    }
                }

                //Check matching status
                if (!hierarchyDepartmentValidationDto.EntityStatusAllow.Contains(EntityStatusEnum.All)
                    && !hierarchyDepartmentValidationDto.EntityStatusAllow.Contains((EntityStatusEnum)currentHierarchyDepartment.Department.EntityStatusId))
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
                }

                //Check if previous department is direct department
                if (hierarchyDepartmentValidationDto.IsDirectParent)
                {
                    if (previousHierarchyDepartment != null &&
                        previousHierarchyDepartment.HDId != currentHierarchyDepartment.ParentId)
                    {
                        throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
                    }
                }
                else
                {
                    if (previousHierarchyDepartment != null && listDepartmentIdsFromTheCurrentToTheTop.All(p => p != currentHierarchyDepartment.DepartmentId))
                    {
                        throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
                    }
                }

                previousHierarchyDepartment = currentHierarchyDepartment;
            }
            return previousHierarchyDepartment;
        }

        public virtual UserEntity ValidateForUpdating(DepartmentEntity parentDepartment, ConexusBaseDto userDto)
        {
            throw new NotImplementedException();
        }

        public virtual UserEntity ValidateMember(int userId)
        {
            var includeProperties = QueryExtension.CreateIncludeProperties<UserEntity>(x => x.UT_Us,
                x => x.Department);
            var userEntity = _userRepository.GetUserByIds(new List<int> { userId }, includeProperties).FirstOrDefault();
            //if (userEntity == null)
            //{
            //    throw new CXValidationException(cxExceptionCodes.ERROR_USER_NOT_FOUND);
            //}
            return userEntity;
        }
    }
}
