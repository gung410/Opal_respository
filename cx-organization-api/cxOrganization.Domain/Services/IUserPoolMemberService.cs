using System.Collections.Generic;
using cxOrganization.Client;

namespace cxOrganization.Domain.Services
{
    public interface IUserPoolMemberService
    {
        List<MembershipDto> AddMembers(int userPoolId, List<MembershipDto> membershipDtos);
        List<MembershipDto> GetMembers(int userPoolId);
        Dictionary<int, int> CountMemberGroupByUserPools(List<int> userPoolIds);
        List<MembershipDto> RemoveMembers(int userPoolId, List<MembershipDto> membershipDtos);
    }
}