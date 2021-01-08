namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_DepartmentDepartmentType
    {
        public string DepartmentTypeId { get; set; }

        public string DepartmentId { get; set; }

        public virtual SAM_Department Department { get; set; }

        public virtual SAM_DepartmentTypes DepartmentType { get; set; }
    }
}
