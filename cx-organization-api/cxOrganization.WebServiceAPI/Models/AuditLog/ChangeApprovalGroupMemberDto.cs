using cxOrganization.Client;

namespace cxOrganization.WebServiceAPI.Models.AuditLog
{
    public class ChangeApprovalGroupMemberDto
    {
        public MemberDto MemberData { get; set; }
        public int UserGroupId { get; set; }
    }
}
