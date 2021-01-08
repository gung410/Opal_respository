using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators.UserTypes
{
    public class UserTypeValidator : IUserTypeValidator
    {
        protected readonly IUserTypeRepository _userTypeRepository;
        protected readonly IWorkContext _workContext;
        public UserTypeValidator(IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IWorkContext workContext)
        {
            _userTypeRepository = userTypeRepository;
            _workContext = workContext;
        }

        public virtual UserTypeEntity ValidateMembership(MemberDto userTypeMemberDto)
        {
            if (_workContext.CurrentCustomerId != userTypeMemberDto.Identity.CustomerId
                || _workContext.CurrentOwnerId != userTypeMemberDto.Identity.OwnerId)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CXTOKEN_INVALID);
            }
            if (!userTypeMemberDto.Identity.Id.HasValue)
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_ID_IS_REQUIRED);

            var userTypeEntity = _userTypeRepository.GetById((int)userTypeMemberDto.Identity.Id);

            if (userTypeEntity == null)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_USER_TYPE_NOT_FOUND, userTypeMemberDto.Identity.Id);

            return userTypeEntity;
        }
    }
}
