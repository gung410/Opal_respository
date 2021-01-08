using System.Collections.Generic;
using cxOrganization.Client;
using cxOrganization.Domain.Validators;

namespace cxOrganization.Domain.Services
{
    public interface ILevelService
    {
        MemberDto AddOrRemoveMember(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId, MemberDto memberDto);
        MemberDto GetMemberAsDepartment(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId, int departmentId);
        MemberDto GetMemberAsTeachingGroup(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId, int teachingGroupId);
        MemberDto GetMemberAsUser(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId, int userId);
        List<MemberDto> GetMembersAsUser(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId);
        List<MemberDto> GetMemberships(HierarchyDepartmentValidationSpecification validationSpecification, string levelExtId);
    }
}
