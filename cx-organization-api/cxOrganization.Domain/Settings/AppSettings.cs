using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Settings
{
    public class AppSettings
    {
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public int OwnerId { get; set; }
        public int SiteId { get; set; }
        public int HDId { get; set; }
        public string AuthorityUrl { get; set; }
        public string ApiBaseUrl { get; set; }
        public string PortalAPI { get; set; }
        public bool IsCrossOrganizationalUnit { get; set; }
        public int CurrentUserId { get; set; }
        public bool CheckSingleUserAccess { get; set; }
        public string FallBackLanguageCode { get; set; }
        public static string ProjectName { get; set; }
        public bool EnableSearchingSSN { get; set; }
        public string ExportStorageFolder { get; set; }
        public string ImportStorageFolder { get; set; }
        public string ExportUserManagementStorageSubFolder { get; set; }
        public string ExportUserAuditEventStorageSubFolder { get; set; }
        public string ExportUserStatisticsStorageSubFolder { get; set; }
        public string ExportApprovingOfficerStorageSubFolder { get; set; }
        public string ExportUserAccountDetailsStorageSubFolder { get; set; }
        public string MassUserCreationStorageSubFolder { get; set; }
        public string ExportPrivilegedUserAccountStorageSubFolder { get; set; }


        public string OPALMainPageLink { get; set; }
        public string SAMLink { get; set; }
        public string PDPMLink { get; set; }
        public string LogoPath { get; set; }
        public string LearnerWebAppLink { get; set; }
        public string LearnerAndroidAppLink { get; set; }
        public string LearnerIOSAppLink { get; set; }

        public string EmailMessageRoutingAction { get; set; }

        public int? TimeZoneOffset { get; set; }
        public string DateTimeDisplayFormat { get; set; }
        public bool EncryptSSN { get; set; }
        public bool AwsKmsEnabled { get; set; }
        public bool HideSSN { get; set; }
        public bool HideDateOfBirth { get; set; }
        public List<string> ApprovingOfficerUserTypeExtIds { get; set; }

        public int? UserMaxPageSize { get; set; }
        public string ExternallyMasteredUserReportDisplayText { get; set; }
        public string NonExternallyMasteredUserReportDisplayText { get; set; }
        public List<string> PrivilegedUserTypeExtIds { get; set; }


        public int LimitUserPageSize(int pageSize)
        {
            return UserMaxPageSize > 0 
                ? Math.Min(UserMaxPageSize.Value, pageSize) 
                : pageSize;
        }
    }
}
