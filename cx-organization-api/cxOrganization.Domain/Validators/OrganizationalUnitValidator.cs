using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class OrganizationalUnitValidator : DepartmentValidator
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdvancedWorkContext _workContext;
        public OrganizationalUnitValidator(IOwnerRepository ownerRepository, 
            ICustomerRepository customerRepository, 
            ILanguageRepository languageRepository,
            IDepartmentRepository departmentRepository, 
            IHierarchyDepartmentRepository hierarchyDepartmentRepository, 
            IAdvancedWorkContext workContext, IUserRepository userRepository) : base(ownerRepository, customerRepository, languageRepository, departmentRepository, hierarchyDepartmentRepository, workContext)
        {
            _userRepository = userRepository;
            _workContext = workContext;
        }
        public override DepartmentEntity Validate(ConexusBaseDto dto)
        {
            var classDto = (OrganizationalUnitDto)dto;
            if (classDto != null && classDto.Identity.Archetype != ArchetypeEnum.OrganizationalUnit)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            DepartmentEntity departmentEntity = base.Validate(dto);

            if (departmentEntity != null && departmentEntity.ArchetypeId != (short)ArchetypeEnum.OrganizationalUnit)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            return departmentEntity;
        }
        public override UserEntity ValidateMemberDtoForUpdating(int departmentId, MemberDto member)
        {

            var includeProperties = QueryExtension.CreateIncludeProperties<UserEntity>(x => x.Department);
            var user = _userRepository.GetUserByIds(userIds: new List<int>() { (int)member.Identity.Id }, includeProperties: includeProperties).FirstOrDefault();

            ValidateMemberDto(user, member);

            if (user.DepartmentId == departmentId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_RESULT_ALREADY_EXISTS);
            }
            var insertedOrganizationalUnit = _departmentRepository.GetById(departmentId);
            if (insertedOrganizationalUnit == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_DEPARTMENT_NOT_FOUND);
            }
            if (insertedOrganizationalUnit.ArchetypeId != (short)ArchetypeEnum.OrganizationalUnit)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }
            return user;

        }
        public override UserEntity ValidateMemberDtoForRemoving(int departmentId, MemberDto member, ref int parentDepartmentId)
        {
            var includeProperties = QueryExtension.CreateIncludeProperties<UserEntity>(x => x.Department);
            var user = _userRepository.GetUserByIds(userIds: new List<int>() { (int)member.Identity.Id }, includeProperties: includeProperties).FirstOrDefault();
            ValidateMemberDto(user, member);
            if (user.DepartmentId != departmentId)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_DEPARTMENT);
            }
            if (user.Department.ArchetypeId != (short)ArchetypeEnum.Class)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_DEPARTMENT_IS_NOT_CLASS);
            }
            var learnerSchool = _departmentRepository.GetParentDepartment(departmentId).FirstOrDefault();
            if (learnerSchool == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_SCHOOL_NOT_FOUND, departmentId);
            }
            parentDepartmentId = learnerSchool.DepartmentId;

            return user;
        }
        private void ValidateMemberDto(UserEntity user, MemberDto member)
        {
            if (user == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_MEMBER_NOT_FOUND);
            }
            ArchetypeEnum enumMemberArchetypeId = (ArchetypeEnum)user.ArchetypeId;
            if (member.Identity.Archetype != enumMemberArchetypeId && (member.Identity.Archetype != ArchetypeEnum.Learner || member.Identity.Archetype != ArchetypeEnum.Employee))
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }

            if ((int)member.EntityStatus.StatusId != user.EntityStatusId)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_USER_PROPERTY_VALIDATION);
            }
           
            if (user.CustomerId != member.Identity.CustomerId || user.CustomerId != _workContext.CurrentCustomerId)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMER_ID_NOT_MATCH);
            }
            
        }
        public override DepartmentEntity ValidateDepartment(int departmentId)
        {
            DepartmentEntity department = _departmentRepository
                .GetDepartmentsByDepartmentIds(departmentIds: new List<int> { departmentId },includeDepartmentTypes: true)
                .FirstOrDefault();
            if (department == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_DEPARTMENT_NOT_FOUND);
            }
            if (department.ArchetypeId != (short)ArchetypeEnum.OrganizationalUnit)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return department;
        }
    }
}
