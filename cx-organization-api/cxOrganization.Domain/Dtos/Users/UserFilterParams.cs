using cxOrganization.Domain.BaseEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Dtos.Users
{
    public class UserFilterParams
    {
        public string Id { get; set; } = null;
        public string UserName { get; set; } = null;
        public string EmailAddress { get; set; } = null;
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public string[] Roles { get; set; } = null;
        public string[] OrgUnitIds { get; set; } = null;
        public bool IsIncludeRole { get; set; } = false;
        public bool IsIncludeOrg { get; set; } = false;
        public bool IsIncludeAccessPlan { get; set; } = false;
        public DateTime? LastLoginDateBefore { get; set; }
        public int? Status { get; set; }
    }
}
