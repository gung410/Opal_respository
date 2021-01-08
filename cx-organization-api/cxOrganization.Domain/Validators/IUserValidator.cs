using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Validators
{
    public interface IUserValidator
    {
        UserEntity Validate(DepartmentEntity parentDepartment,
            ConexusBaseDto dto,
            int? currentOwnerId = null,
            int? currentCustomerId = null);
        void ValidateMember(MemberDto member);
        HierarchyDepartmentEntity ValidateHierarchyDepartment(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto);
        UserEntity ValidateForUpdating(DepartmentEntity parentDepartment, ConexusBaseDto userDto);
        UserEntity ValidateMember(int userId);
    }
}
