using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class DataOwnerValidator : DepartmentValidator
    {
        public DataOwnerValidator(IDepartmentRepository departmentRepository,
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
            var dataOwnerDto = (DataOwnerDto)dto;
            if (dataOwnerDto != null && dataOwnerDto.Identity.Archetype != ArchetypeEnum.DataOwner)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            DepartmentEntity departmentEntity = base.Validate(dto);
            //if (dataOwnerDto.Identity.Id > 0)
            //{
            //    if (!departmentEntity.H_D.Any(t => t.Parent.DepartmentId == dataOwnerDto.ParentDepartmentId))
            //    {

            //    }
            //}
            if (departmentEntity != null && departmentEntity.ArchetypeId != (short)ArchetypeEnum.DataOwner)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            return departmentEntity;
        }
    }
}
