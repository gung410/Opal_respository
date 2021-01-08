using System.Collections.Generic;
using cxOrganization.Client;
using cxOrganization.Domain.Validators;

namespace cxOrganization.Domain.Services
{
    public interface IClassMemberService
    {
        MemberDto AddOrRemoveMember(HierarchyDepartmentValidationSpecification validationSpecification, int schoolId, int classId, MemberDto memberDto);
        MemberDto GetMember(HierarchyDepartmentValidationSpecification validationSpecification, int memberId);
        List<MemberDto> GetMembers(HierarchyDepartmentValidationSpecification validationSpecification, int classId);
        MemberDto UpdateMember(HierarchyDepartmentValidationSpecification validationSpecification, MemberDto memberDto);
        List<MemberDto> GetClassMemberShip(HierarchyDepartmentValidationSpecification validationSpecicication, int schoolId, int classId);
        MemberDto AddLearnerToClass(int learnerId, MemberDto classDto);
        MemberDto RemoveLearnerFromClass(int learnerId, MemberDto classDto);
        MemberDto AddTypeToClass(int departmentId, MemberDto departmentTypeDto, bool isUniqueDepartmentType = false);
        MemberDto RemoveDepartmentTypeClass(int departmentId, MemberDto departmentTypeDto);
        List<MemberDto> GetClassMemberships(int classId);
        Dictionary<string, List<MemberDto>> GetClassesMemberships(List<string> classExtIds, int ownerId, int customerId);
    }
}
