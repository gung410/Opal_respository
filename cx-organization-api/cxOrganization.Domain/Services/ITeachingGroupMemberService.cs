using System.Collections.Generic;
using cxOrganization.Client;
using cxOrganization.Domain.Validators;

namespace cxOrganization.Domain.Services
{
    public interface ITeachingGroupMemberService
    {
        MemberDto AddMember(HierarchyDepartmentValidationSpecification validationSpecification, int teachingGroupId, MemberDto memberDto);
        List<MemberDto> GetMembers(HierarchyDepartmentValidationSpecification validationSpecification, int teachingGroupId);
        MemberDto GetMember(HierarchyDepartmentValidationSpecification validationSpecification, int teachingGroupId, int memberId);
        List<MemberDto> GetUserGroupMemberShip(HierarchyDepartmentValidationSpecification validationSpecification, int userGroupId);
        List<MemberDto> GetTeachingGroupMemberships(int teachingGroupId);
        MemberDto UpdateTeachingGroupLevel(int teachinggroupId, MemberDto levelDto);
        MemberDto RemoveTeachingGroupLevel(int teachinggroupId, MemberDto levelDto);
    }
}
