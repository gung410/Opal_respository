using System.Collections.Generic;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.Users;

namespace cxOrganization.Domain.Services.Reports
{
    public class ApprovingOfficerInfo: UserGenericDto
    {
        public int PrimaryApprovalMemberCount { get; set; }
        public int AlternativeApprovalMemberCount { get; set; }

    }
}
