using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class CompanyValidator : DepartmentValidator
    {
        public CompanyValidator(IDepartmentRepository departmentRepository,
            IOwnerRepository ownerRepository,
            ICustomerRepository customerRepository,
            ILanguageRepository languageRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IAdvancedWorkContext workContext)
            : base(ownerRepository, customerRepository,
                 languageRepository, departmentRepository, hierarchyDepartmentRepository, workContext)
        {
        }

        public override DepartmentEntity Validate(ConexusBaseDto dto)
        {
            var companyDto = (CompanyDto)dto;
            if (companyDto != null && companyDto.Identity.Archetype != ArchetypeEnum.Company)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            DepartmentEntity departmentEntity = base.Validate(dto);

            if (departmentEntity != null && departmentEntity.ArchetypeId != (short)ArchetypeEnum.Company)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return departmentEntity;
        }

    }
}
