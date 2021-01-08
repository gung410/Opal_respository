using cxOrganization.Client.Departments;
using cxOrganization.Domain.Dtos.Departments;

namespace cxOrganization.WebServiceAPI.Models
{
    public class TopHierarchyDepartmentIdentityDto : HierachyDepartmentIdentityDto
    {
        public BasicHierarchyInfo DefaultHierarchyDepartment { get; set; }
    }
}
