using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class SchoolOwnerValidator : DepartmentValidator
    {
        public SchoolOwnerValidator(IDepartmentRepository departmentRepository,
            IOwnerRepository ownerRepository,
            ICustomerRepository customerRepository,
            ILanguageRepository languageRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IWorkContext workContext)
            : base(ownerRepository, customerRepository, languageRepository, departmentRepository, hierarchyDepartmentRepository, workContext)
        {
        }
        public override DepartmentEntity Validate(ConexusBaseDto dto)
        {
            var schoolOwnerDto = (SchoolOwnerDto)dto;
            if (schoolOwnerDto != null && schoolOwnerDto.Identity.Archetype != ArchetypeEnum.SchoolOwner)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            DepartmentEntity departmentEntity = base.Validate(dto);

            if (departmentEntity != null && departmentEntity.ArchetypeId != (short)ArchetypeEnum.SchoolOwner)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return departmentEntity;
        }
    }
}
