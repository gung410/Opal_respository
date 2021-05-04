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
    public class SchoolValidator : DepartmentValidator
    {
        public SchoolValidator(IDepartmentRepository departmentRepository,
            IOwnerRepository ownerRepository,
            ICustomerRepository customerRepository,
            ILanguageRepository languageRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IAdvancedWorkContext workContext)
            : base(ownerRepository, customerRepository, languageRepository, departmentRepository, hierarchyDepartmentRepository, workContext)
        {
        }

        public override DepartmentEntity Validate(ConexusBaseDto dto)
        {
            var schoolDto = (SchoolDto)dto;
            if (schoolDto != null && schoolDto.Identity.Archetype != ArchetypeEnum.School)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            DepartmentEntity departmentEntity = base.Validate(dto);

            if (departmentEntity != null && departmentEntity.ArchetypeId != (short)ArchetypeEnum.School)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return departmentEntity;
        }
        public override UserEntity ValidateMemberDtoForUpdating(int departmentId, MemberDto member)
        {
            //var school = _departmentRepository.GetDepartments(departmentIds: new List<int>() { departmentId }, includeProperties: null).FirstOrDefault();

            return base.ValidateMemberDtoForUpdating(departmentId, member);
        }
    }
}
