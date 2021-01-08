using cxOrganization.Client;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Validators
{
    public interface IUGMemberValidator
    {
        UGMemberEntity Validate(MembershipDto membershipDto);

    }
}
