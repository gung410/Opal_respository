
namespace cxOrganization.Domain.Dtos.Departments
{
    public class HierarchyInfo : BasicHierarchyInfo
    {
        public string DepartmentPath { get; set; }
    }

    public class BasicHierarchyInfo
    {
        public int HierarchyId { get; set; }
        public int HdId { get; set; }
        public int? ParentHdId { get; set; }
        public string Path { get; set; }
        public int DepartmentId { get; set; }
    }
}
