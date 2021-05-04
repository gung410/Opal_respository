using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class EmployeeValidator : UserValidator
    {
        public EmployeeValidator(IUserRepository userRepository,
            IAdvancedWorkContext workContext,
            ICustomerRepository customerRepository,
            IOwnerRepository ownerRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository)
            : base(userRepository, workContext, customerRepository, ownerRepository, hierarchyDepartmentRepository)
        {
        }

        public override UserEntity Validate(DepartmentEntity parentDepartment,
            ConexusBaseDto dto,
            int? currentOwnerId = null,
            int? currentCustomerId = null)
        {
            //Archetype must be Employee
            if (dto.Identity.Archetype != ArchetypeEnum.Employee)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            UserEntity userEntity = base.Validate(parentDepartment, dto, currentOwnerId, currentCustomerId);
            var userDto = (EmployeeDto)dto;

            //User.ArchetypeId must be Employee
            if (userEntity != null && userEntity.ArchetypeId != (short)ArchetypeEnum.Employee)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            if (userDto.EmployerDepartmentId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_PARENTDEPARTMENTID_REQUIRED);
            }
            if (parentDepartment == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
            }
            else if (parentDepartment.ArchetypeId == (int)ArchetypeEnum.Class)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            return userEntity;
        }
        public override UserEntity ValidateMember(int userId)
        {
            var user = base.ValidateMember(userId);
            if (!user.ArchetypeId.HasValue ||user.ArchetypeId != (short)ArchetypeEnum.Employee)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }
            return user;
        }
    }
}
