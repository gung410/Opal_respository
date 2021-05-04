using cxOrganization.Client;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators.UserTypes
{
    public class LevelValidator : UserTypeValidator
    {
        public LevelValidator(IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IAdvancedWorkContext workContext) : base(userRepository, userTypeRepository, workContext)
        {
        }
        public override UserTypeEntity ValidateMembership(MemberDto userTypeMemberDto)
        {
            if (_workContext.CurrentCustomerId != userTypeMemberDto.Identity.CustomerId
                || _workContext.CurrentOwnerId != userTypeMemberDto.Identity.OwnerId)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CXTOKEN_INVALID);
            }
            if (userTypeMemberDto.Identity.Archetype != ArchetypeEnum.Level)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);

            if (!userTypeMemberDto.Identity.Id.HasValue)
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);

            var userTypeEntity = _userTypeRepository.GetUserTypeByExtId(userTypeMemberDto.Identity.Id.ToString());

            if (userTypeEntity == null)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_USER_TYPE_NOT_FOUND, userTypeMemberDto.Identity.Id.ToString());

            if (!userTypeEntity.ArchetypeId.HasValue || userTypeEntity.ArchetypeId != (int)userTypeMemberDto.Identity.Archetype)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_MEMBER_ARCHETYPE_INCORRECTED);

            return userTypeEntity;
        }
    }
}
