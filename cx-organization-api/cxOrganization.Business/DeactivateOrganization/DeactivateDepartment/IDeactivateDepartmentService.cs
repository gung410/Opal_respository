namespace cxOrganization.Business.DeactivateOrganization.DeactivateDepartment
{
    public interface IDeactivateDepartmentService
    {
        DeactivateDepartmentsResultDto DeactivateDepartments(DeactivateDepartmentsDto deactivateDepartmentDto);
    }
}