namespace cxOrganization.Domain.Dtos.DataHub
{
    public static class AuditLogEvents
    {
        public const string UserCreated = "cx-organization-api.crud.created.employee";
        public const string UserUpdated = "cx-organization-api.crud.updated.employee";
        public const string EntityStatusChanged = "cx-organization-api.crud.entitystatus_changed.employee";
        public const string ApprovalGroupMemberAdded = "cx-organization-api.crud.user_membership_created.approvalgroup";
        public const string ApprovalGroupMemberRemoved = "cx-organization-api.crud.user_membership_deleted.approvalgroup";
        public const string LoginFailed = "cxid.system_warn.locked.user";
    }
}
