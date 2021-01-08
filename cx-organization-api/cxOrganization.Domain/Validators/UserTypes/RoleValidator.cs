using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators.UserTypes
{
    public class RoleValidator : UserTypeValidator
    {
        public RoleValidator(IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IWorkContext workContext) : base(userRepository, userTypeRepository, workContext)
        {
        }
        public override UserTypeEntity ValidateMembership(MemberDto userTypeMemberDto)
        {
            if (userTypeMemberDto.Identity.Archetype != ArchetypeEnum.Role)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_USERTYPE_IS_INCORRECT);

            var userTypeEntity = base.ValidateMembership(userTypeMemberDto);

            return userTypeEntity;
        }
    }
}
