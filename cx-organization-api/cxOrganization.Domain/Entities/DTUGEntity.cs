namespace cxOrganization.Domain.Entities
{
    public class DTUGEntity
    {
        public DepartmentTypeEntity DepartmentType { get; set; }
        public UserGroupEntity UserGroup { get; set; }
        public int DepartmentTypeId { get; set; }
        public int UserGroupId { get; set; }
    }
}
