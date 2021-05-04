namespace cxOrganization.Domain.DomainEnums
{
    public static class OrganizationPermissionKeys
    {
        // USER MANAGEMENT
        public const string SeeMenuUserManagement = "OrganizationSpa.UserManagement.SeeMenu";
        public const string SingleUserCreation = "OrganizationSpa.UserManagement.SingleUserCreation";
        public const string MassUserCreation = "OrganizationSpa.UserManagement.MassUserCreation";
        public const string ExportUsers = "OrganizationSpa.UserManagement.ExportUsers";
        public const string BasicUserAccountsManagement = "OrganizationSpa.UserManagement.BasicUserAccountsManagement";
        public const string AdvancedUserAccountsManagement = "OrganizationSpa.UserManagement.AdvancedUserAccountsManagement";

        // USER MANAGEMENT - PENDING LEVEL 1
        public const string ViewPending1st = "OrganizationSpa.UserManagement.Pending1st.View";
        public const string EndorsePending1st = "OrganizationSpa.UserManagement.Pending1st.Endorse";
        public const string RejectPending1st = "OrganizationSpa.UserManagement.Pending1st.Reject";
        public const string EditPending1st = "OrganizationSpa.UserManagement.Pending1st.Edit";
        public const string ApprovePending1st = "OrganizationSpa.UserManagement.Pending1st.Approve";

        // USER MANAGEMENT - PENDING LEVEL 2
        public const string ViewPending2nd = "OrganizationSpa.UserManagement.Pending2nd.View";
        public const string ApprovePending2nd = "OrganizationSpa.UserManagement.Pending2nd.Approve";
        public const string RejectPending2nd = "OrganizationSpa.UserManagement.Pending2nd.Reject";
        public const string RequestSpecialApprovalPending2nd = "OrganizationSpa.UserManagement.Pending2nd.RequestSpecialApproval";
        public const string EditPending2nd = "OrganizationSpa.UserManagement.Pending2nd.Edit";

        // USER MANAGEMENT - PENDING LEVEL 3
        public const string ViewPendingSpecial = "OrganizationSpa.UserManagement.PendingSpecial.View";
        public const string EditPendingSpecial = "OrganizationSpa.UserManagement.PendingSpecial.Edit";
        public const string ApprovePendingSpecial = "OrganizationSpa.UserManagement.PendingSpecial.Approve";
        public const string RejectPendingSpecial = "OrganizationSpa.UserManagement.PendingSpecial.Reject";

        // USER MANAGEMENT - OTHER PLACE OF WORK
        public const string ViewOtherPlaceOfWork = "OrganizationSpa.UserManagement.OtherPlaceOfWork.View";
        public const string ChangePlaceOfWorkInOtherPlaceOfWork = "OrganizationSpa.UserManagement.OtherPlaceOfWork.ChangePlaceOfWork";
        public const string RejectInOtherPlaceOfWork = "OrganizationSpa.UserManagement.OtherPlaceOfWork.Reject";
        public const string EditOtherPlaceOfWork = "OrganizationSpa.UserManagement.OtherPlaceOfWork.Edit";
        public const string CreateOrganisationUnitInOtherPlaceOfWork = "OrganizationSpa.UserManagement.OtherPlaceOfWork.CreateOrganisationUnit";

        // ORGANISATION MANAGEMENT
        public const string SeeMenuOrganisationManagement = "OrganizationSpa.OrganisationManagement.SeeMenu";
        public const string CRUDinOrganisationManagement = "OrganizationSpa.OrganisationManagement.CRUD";

        // USER GROUP MANAGEMENT
        public const string SeeMenuUserGroupManagement = "OrganizationSpa.UserGroupManagement.SeeMenu";
        public const string CUDinUserGroupManagement = "OrganizationSpa.UserGroupManagement.CUD";

        // BROADCAST
        public const string SeeMenuBroadcastMessages = "OrganizationSpa.BroadcastMessages.SeeMenu";
        public const string CRUDinBroadcastMessages = "OrganizationSpa.BroadcastMessages.CRUD";

        // REPORT
        public const string SeeMenuReports = "OrganizationSpa.Reports.SeeMenu";

        // SYSTEM_AUDIT_LOG
        public const string SeeMenuSystemAuditLog = "OrganizationSpa.SystemAuditLog.SeeMenu";

        // METADATA
        public const string SuggestMetadataChange = "OrganizationSpa.Metadata.SuggestMetadataChange";
        public const string MetadataViewPending1st = "OrganizationSpa.Metadata.Pending1st.View";
        public const string MetadataApprovePending1st = "OrganizationSpa.Metadata.Pending1st.Approve";
        public const string MetadataRejectPending1st = "OrganizationSpa.Metadata.Pending1st.Reject";
        public const string MetadataEditPending1st = "OrganizationSpa.Metadata.Pending1st.Edit";
        public const string MetadataViewPending2nd = "OrganizationSpa.Metadata.Pending2nd.View";
        public const string MetadataApprovePending2nd = "OrganizationSpa.Metadata.Pending2nd.Approve";
        public const string MetadataRejectPending2nd = "OrganizationSpa.Metadata.Pending2nd.Reject";
        public const string MetadataEditPending2nd = "OrganizationSpa.Metadata.Pending2nd.Edit";
        public const string MetadataViewApprovedList = "OrganizationSpa.Metadata.ApprovedList.View";
        public const string MetadataMarkAsCompleteInApprovedList = "OrganizationSpa.Metadata.ApprovedList.MarkAsComplete";
        public const string MetadataViewRejectedList = "OrganizationSpa.Metadata.RejectedList.View";
        public const string MetadataViewCompletedList = "OrganizationSpa.Metadata.CompletedList.View";

        // PERMISSIONS
        public const string SeeMenuPermissions = "OrganizationSpa.Permissions.SeeMenu";
    }
}
