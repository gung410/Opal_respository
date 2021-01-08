using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.WebServiceAPI.Models
{
    public class GetUserHierarchyInfoParameters
    {
        public List<int> UserIds { get; set; }
        public List<string> ExtIds { get; set; }
        public List<string> Emails { get; set; }
        public List<EntityStatusEnum> EntityStatuses { get; set; }
        public List<int> DepartmentIds { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string OrderBy { get; set; }

    }
}