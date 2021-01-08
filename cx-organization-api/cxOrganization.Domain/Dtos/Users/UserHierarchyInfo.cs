using cxOrganization.Domain.Dtos.Departments;

namespace cxOrganization.Domain.Dtos.Users
{
    public class UserHierarchyInfo
    {
        public int UserId { get; set; }
        public string UserCxId { get; set; }
        public int DepartmentId { get; set; }
        public HierarchyInfo HierarchyInfo { get; set; }

    }
 
}
