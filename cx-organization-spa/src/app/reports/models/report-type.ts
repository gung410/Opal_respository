export enum ReportType {
  UserAccountDetails = 'UserAccountDetails',
  AccountStatistics = 'accountStatistics',
  PrivilegedAccounts = 'privilegedAccounts',
  OfficerPWCMeeting = 'OfficerPWCMeeting',
  AllUsers = 'AllUsers',
  UserAccountStatus = 'UserAccountStatus'
}

/**
 * The list of report types using iframe to load the report.
 */
export const reportTypeUsingIframe = [
  ReportType.OfficerPWCMeeting.toString(),
  ReportType.AllUsers.toString(),
  ReportType.UserAccountStatus.toString()
];
