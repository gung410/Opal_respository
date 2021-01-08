using cxOrganization.Client;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Validators.UserTypes
{
    public interface IUserTypeValidator
    {
        /// <summary>
        /// Validate usertype memberdto
        /// Check matching cxtoken
        /// </summary>
        /// <param name="userTypeMemberDto">Holding user type info</param>
        UserTypeEntity ValidateMembership(MemberDto userTypeMemberDto);

    }
}
