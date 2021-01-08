using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.WebServiceAPI.Models
{
    public class GetUserBasicInfoParameters
    {
        public List<int> UserIds { get; set; }
        public List<string> ExtIds { get; set; }
        public List<string> Emails { get; set; }
        public List<EntityStatusEnum> EntityStatuses { get; set; }
        public string SearchKey { get; set; }
        public List<int> DepartmentIds { get; set; }
        public bool? ExternallyMastered { get; set; }
        public List<int> UserTypeIds { get; set; }
        public List<string> SystemRolePermissions { get; set; }
        public List<List<int>> MultiUserTypeFilters { get; set; }
        public List<List<string>> MultiUserTypeExtIdFilters { get; set; }
        public List<string> UserTypeExtIds { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string OrderBy { get; set; }

        public bool GetFullIdentity { get; set; }
        public bool GetEntityStatus { get; set; }
        public List<int> UserGroupIds { get; set; }
        public List<int> ExceptUserIds { get; set; }


    }
}