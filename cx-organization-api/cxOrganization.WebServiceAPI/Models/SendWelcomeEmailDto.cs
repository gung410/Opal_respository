using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.WebServiceAPI.Models
{
    public class SendWelcomeEmailDto
    {
        public SendWelcomeEmailUserFilterDto UserFilter { get; set; }
    }

    public class SendWelcomeEmailUserFilterDto
    {
        public List<int> UserIds { get; set; }
        public List<string> ExtIds { get; set; }
        public List<int> ParentDepartmentIds { get; set; }
        public List<string> Emails { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string OrderBy { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? LastUpdatedBefore { get; set; }
        public DateTime? LastUpdatedAfter { get; set; }
        public DateTime? ExpirationDateAfter { get; set; }
        public DateTime? ExpirationDateBefore { get; set; }
        public List<bool> ExternallyMasteredValues { get; set; }
        public List<EntityStatusEnum> UserEntityStatuses { get; set; }
    }
}
