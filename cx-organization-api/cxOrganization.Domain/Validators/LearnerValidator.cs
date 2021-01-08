using cxOrganization.Client;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class LearnerValidator : UserValidator
    {
        public LearnerValidator(IUserRepository userRepository,
            IWorkContext workContext,
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
            //Archetype must be learner
            if (dto.Identity.Archetype != ArchetypeEnum.Learner)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            UserEntity userEntity = base.Validate(parentDepartment, dto, currentOwnerId);
            var userDto = (LearnerDto)dto;

            //User.ArchetypeId must be Learner
            if (userEntity != null && userEntity.ArchetypeId != (short)ArchetypeEnum.Learner)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            if (userDto.ParentDepartmentId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_PARENTDEPARTMENTID_REQUIRED);
            }
            if (parentDepartment == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
            }
            //not allow change user department when user stay in class
            else if (parentDepartment.ArchetypeId == (int)ArchetypeEnum.Class)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_DEPARTMENT_IS_NOT_SCHOOLOWNERANDSCHOOL);
            }

            return userEntity;
        }

        public override void ValidateMember(MemberDto member)
        {
            if (member.Identity.Archetype != ArchetypeEnum.Learner)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }
        }
        public override UserEntity ValidateMember(int userId)
        {
            var user = base.ValidateMember(userId);
            if (!user.ArchetypeId.HasValue ||user.ArchetypeId != (short)ArchetypeEnum.Learner)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }
            return user;
        }
    }
}
