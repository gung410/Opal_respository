using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class DepartmentValidator : IDepartmentValidator
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILanguageRepository _languageRepository;
        protected readonly IDepartmentRepository _departmentRepository;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IWorkContext _workContext;

        public DepartmentValidator(IOwnerRepository ownerRepository,
            ICustomerRepository customerRepository,
            ILanguageRepository languageRepository,
            IDepartmentRepository departmentRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IWorkContext workContext)
        {
            _ownerRepository = ownerRepository;
            _customerRepository = customerRepository;
            _languageRepository = languageRepository;
            _departmentRepository = departmentRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _workContext = workContext;
        }

        public virtual DepartmentEntity Validate(ConexusBaseDto dto)
        {
            DepartmentEntity departmentEntity = null;
            var departmentDto = (DepartmentDtoBase)dto;

            //Entity status must be set
            if (departmentDto.EntityStatus.StatusId == EntityStatusEnum.Unknown)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_STATUS_ID_REQUIRED);
            }

            //If an object is "Locked" then the ExtId is mandatory to be set with a value
            if (departmentDto.EntityStatus.ExternallyMastered && string.IsNullOrEmpty(departmentDto.Identity.ExtId))
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_IDENTITY_LOCKED_EXTID_MANDATORY);
            }

            //Validate OwnerId
            if (departmentDto.Identity.OwnerId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_OWNERID_REQUIRED);
            }
            var owner = _ownerRepository.GetById(departmentDto.Identity.OwnerId);

            if (owner == null)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_OWNERID_NOT_FOUND);
            }

            //Validate CustomerId
            if (departmentDto.Identity.CustomerId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_REQUIRED);
            }
            if (departmentDto.Identity.CustomerId != _workContext.CurrentCustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_ID_NOT_MATCH);
            }
            var customer = _customerRepository.GetById(departmentDto.Identity.CustomerId);
            if (customer == null)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_NOT_FOUND);
            }

            if (departmentDto.LanguageId > 0)
            {
                var language = _languageRepository.GetById(departmentDto.LanguageId);
                if (language == null)
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_LANGUAGEID_NOT_FOUND);
                }
            }
            var departmentId = (int?)departmentDto.Identity.Id;
            var departmentEntities = _departmentRepository.GetDepartmentsByIdOrExtId(departmentId, departmentDto.Identity.ExtId);
            //Check duplicate ExtId
            if (!string.IsNullOrEmpty(departmentDto.Identity.ExtId))
            {
                var departmentsFilterByExtId = departmentEntities.Where(p => p.ExtId.ToLower() 
                == departmentDto.Identity.ExtId.ToLower()).ToList();
                if (departmentsFilterByExtId.Any(p => ((p.DepartmentId != departmentDto.Identity.Id && departmentDto.Identity.Id > 0) || departmentDto.Identity.Id == 0) 
                                                    && p.CustomerId == departmentDto.Identity.CustomerId
                                                    && !p.Deleted.HasValue
                                                    && p.ArchetypeId == (short)departmentDto.Identity.Archetype))
                {
                    throw new CXValidationException(cxExceptionCodes.VALIDATION_DEPARTMENT_EXTID_EXISTS_CUSTOMER);
                }
            }
            if (departmentId > 0)
            {
                departmentEntity = departmentEntities.FirstOrDefault(p => p.DepartmentId == departmentDto.Identity.Id);
                if (!departmentEntity.H_D.Any(t => t.Parent.DepartmentId == departmentDto.ParentDepartmentId))
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_BELONG_TO_PARENT_DEPARTMENT);
                }
                if (departmentEntity == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("Id : {0}", departmentId));
                }
            }

            return departmentEntity;
        }

        public HierarchyDepartmentEntity ValidateHierarchyDepartment(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto)
        {
            var departmentIds = hierarchyDepartmentValidationDto.HierarchyDepartments.Select(p => p.Key).ToArray();
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
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
                    if (currentHierarchyDepartment.Department.ArchetypeId != (int)archetypeShouldMatch)
                    {
                        throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
                    }
                }
                if (hierarchyDepartmentValidationDto.IsNullArchetype)
                {
                    if (currentHierarchyDepartment.Department.ArchetypeId != null)
                    {
                        throw new CXValidationException(cxExceptionCodes.VALIDATION_PARENTDEPARTMENT_IS_INCORRECT);
                    }
                }
                if (hierarchyDepartmentValidationDto.IsNotInArchetypes.Any())
                {
                    if (hierarchyDepartmentValidationDto.IsNotInArchetypes.Any(t => (int)t == currentHierarchyDepartment.Department.ArchetypeId))
                    {
                        throw new CXValidationException(cxExceptionCodes.VALIDATION_PARENTDEPARTMENT_IS_INCORRECT);
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

       
        public virtual UserEntity ValidateMemberDtoForUpdating(int departmentId, MemberDto member)
        {
            throw new NotImplementedException();
        }

        public virtual UserEntity ValidateMemberDtoForRemoving(int departmentId, MemberDto member, ref int parentDepartmentId)
        {
            throw new NotImplementedException();
        }

        public virtual DepartmentEntity ValidateDepartment(int departmentId)
        {
            throw new NotImplementedException();
        }
    }
}
