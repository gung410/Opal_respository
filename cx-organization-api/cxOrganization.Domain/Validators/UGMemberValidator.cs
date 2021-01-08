using System.Collections;
using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Domain.Validators
{
    public class UGMemberValidator : IUGMemberValidator
    {
        private readonly IUGMemberRepository _uGMemberRepository;
        public UGMemberValidator(IUGMemberRepository uGMemberRepository)
        {
            _uGMemberRepository = uGMemberRepository;
        }
        public UGMemberEntity Validate(MembershipDto membershipDto)
        {
            if(membershipDto.validFrom.HasValue 
               && membershipDto.ValidTo.HasValue
               && membershipDto.validFrom > membershipDto.ValidTo)
            {
                throw new CXValidationException(cxExceptionCodes.VALID_FROM_AND_VALID_TO_ARE_INCORRECT);
            }

            UGMemberEntity uGMemberEntity = null;

            if (membershipDto.Identity.Id.HasValue)
            {
                uGMemberEntity = _uGMemberRepository.GetById(membershipDto.Identity.Id);

                if(uGMemberEntity == null)
                    throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);

                if (!StructuralComparisons.StructuralEqualityComparer.Equals(membershipDto.EntityStatus.EntityVersion, uGMemberEntity.EntityVersion))
                    throw new CXValidationException(cxExceptionCodes.ERROR_ENTITY_VERSION_INCORRECTED);
            }

            return uGMemberEntity;
        }
    }
}
