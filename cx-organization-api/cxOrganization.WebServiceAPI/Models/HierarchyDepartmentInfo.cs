using cxOrganization.Client.Departments;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.WebServiceAPI.Models
{
    public class HierarchyDepartmentInfo
    {
        public IdentityDto Identity { get; set; }
        public string Name { get; set; }
        public int? ParentDepartmentId { get; set; }
        public static HierarchyDepartmentInfo CreateFrom(HierachyDepartmentIdentityDto hierachyDepartmentIdentityDto)
        {
            return new HierarchyDepartmentInfo
            {
                Identity = hierachyDepartmentIdentityDto.Identity,
                Name = hierachyDepartmentIdentityDto.DepartmentName,
                ParentDepartmentId = hierachyDepartmentIdentityDto.ParentDepartmentId
            };
        }
    }
}
