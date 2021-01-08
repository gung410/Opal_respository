using System.Collections.Generic;
using cxOrganization.Client;
using cxOrganization.Domain.Validators;

namespace cxOrganization.Domain.Services
{
    public interface IApprovalGroupMemberService
    {
        List<MemberDto> GetApprovalGroupMemberships(int approvalgroupid);
        List<MembershipDto> ProcessAddUserGroupUserMembership(List<int> employeeIds, MemberDto memberDtos);
        List<MembershipDto> ProcessRemoveUserGroupUserMembership(List<int> employeeIds, MemberDto memberDto);
    }
}
