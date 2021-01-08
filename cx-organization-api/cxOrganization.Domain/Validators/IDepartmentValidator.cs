using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Validators
{
    public interface IDepartmentValidator
    {
        DepartmentEntity Validate(ConexusBaseDto dto);

        HierarchyDepartmentEntity ValidateHierarchyDepartment(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto);
        UserEntity ValidateMemberDtoForUpdating(int departmentId, MemberDto member);
        UserEntity ValidateMemberDtoForRemoving(int departmentId, MemberDto member, ref int parentDepartmentId);
        DepartmentEntity ValidateDepartment(int departmentId);
    }
}
