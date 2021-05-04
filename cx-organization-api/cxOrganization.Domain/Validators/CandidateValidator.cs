using cxOrganization.Client;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class CandidateValidator : UserValidator
    {
        IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        public CandidateValidator(IUserRepository userRepository,
            IAdvancedWorkContext workContext,
            ICustomerRepository customerRepository,
            IOwnerRepository ownerRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository)
            : base(userRepository, workContext, customerRepository, ownerRepository, hierarchyDepartmentRepository)
        {
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
        }

        public override UserEntity Validate(DepartmentEntity parentDepartment,
            ConexusBaseDto dto,
            int? currentOwnerId = null,
            int? currentCustomerId = null)
        {
            //Archetype must be candidate
            if (dto.Identity.Archetype != ArchetypeEnum.Candidate)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_IS_NOT_SUPPORTED);
            }

            UserEntity userEntity = base.Validate(parentDepartment, dto, currentOwnerId, currentCustomerId);
            var userDto = (CandidateDto)dto;

            //User.ArchetypeId must be candidate
            if (userEntity != null && userEntity.ArchetypeId != (short)ArchetypeEnum.Candidate)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }

            if (userDto.ParentDepartmentId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
            }
            if (parentDepartment == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_PARENTDEPARTMENTID_NOT_FOUND);
            }

            return userEntity;
        }

        public override void ValidateMember(MemberDto member)
        {
            if (member.Identity.Archetype != ArchetypeEnum.Candidate)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);
            }
        }
        public override UserEntity ValidateForUpdating(DepartmentEntity parentDepartment, ConexusBaseDto userDto)
        {
            var userEntity= Validate(parentDepartment, userDto);
            
            return userEntity;

            //return base.Validate(parentHd, parentDepartment, userDto);
        }
        public override UserEntity ValidateMember(int userId)
        {
            var user = base.ValidateMember(userId);
            if (user.ArchetypeId !=(short) ArchetypeEnum.Candidate)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CANDIDATE_ID_NOT_FOUND, userId);
            }
            return user;
        }
    }
}
