using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Validators
{
    interface IUserPoolValidator : IUserGroupValidator
    {
        UserGroupEntity ValidateMembership(MembershipDto membershipDto);
        UserGroupEntity ValidateMemberships(int userPoolId, List<MembershipDto> membershipDtos);
    }
}
