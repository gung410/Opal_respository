using System.Collections.Generic;
using cxOrganization.Client;

namespace cxOrganization.Domain.Services
{
    public interface ICandidatePoolMemberService
    {
        /// <summary>
        /// Adding member
        /// </summary>
        /// <param name="candidatePoolId"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        MemberDto AddMember(int candidatePoolId, MemberDto memberDto);
        /// <summary>
        /// Get members
        /// </summary>
        /// <param name="candidatePoolId"></param>
        /// <returns></returns>
        List<MemberDto> GetMembers(int candidatePoolId);
        /// <summary>
        /// Get member
        /// </summary>
        /// <param name="candidatePoolId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        MemberDto GetMember(int candidatePoolId, int memberId);
        /// <summary>
        /// get user group membership
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        List<MemberDto> GetUserGroupMemberShip(int userGroupId);
    }
}
