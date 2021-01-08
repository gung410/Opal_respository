using System.Linq;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class CandidateDepartmentValidator : DepartmentValidator
    {
        public CandidateDepartmentValidator(IDepartmentRepository departmentRepository,
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
            var candidateDepartmentDto = (CandidateDepartmentDto)dto;
            if (candidateDepartmentDto != null && candidateDepartmentDto.Identity.Archetype != ArchetypeEnum.CandidateDepartment)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            DepartmentEntity departmentEntity = base.Validate(dto);
            if (candidateDepartmentDto.Identity.Id > 0)
            {
                if (!departmentEntity.H_D.Any(t => t.Parent.DepartmentId == candidateDepartmentDto.ParentDepartmentId))
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_BELONG_TO_PARENT_DEPARTMENT);
                }
            }

            if (departmentEntity != null && departmentEntity.ArchetypeId != (short)ArchetypeEnum.CandidateDepartment)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return departmentEntity;
        }

    }
}
