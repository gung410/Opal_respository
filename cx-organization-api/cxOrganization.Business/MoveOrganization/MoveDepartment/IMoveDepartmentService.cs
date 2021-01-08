namespace cxOrganization.Business.MoveOrganization.MoveDepartment
{
    public interface IMoveDepartmentService
    {
        MoveDepartmentsResultDto MoveDepartments(MoveDepartmentsDto moveDepartmentDto);
    }
}