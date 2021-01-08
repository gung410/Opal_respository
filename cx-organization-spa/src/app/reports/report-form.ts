import { ReportType, reportTypeUsingIframe } from './models/report-type';

// tslint:disable:variable-name

export const AuditReportFields = [
  {
    value: 'typeOfOrganization',
    text: 'Type of Organisation'
  },
  {
    value: 'departmentName',
    text: 'Name of Organisation'
  },
  {
    value: 'fullName',
    text: 'Full Name'
  },
  {
    value: 'serviceScheme',
    text: 'Service Scheme'
  },
  {
    value: 'designation',
    text: 'Designation'
  },
  {
    value: 'emailAddress',
    text: 'Email Address'
  },
  {
    value: 'systemRoles',
    text: 'System Roles'
  },
  {
    value: 'onboardingStatus',
    text: 'Onboarding Status (Yes/No)'
  },
  {
    value: 'lastLoginDate',
    text: 'Last Login Date'
  },
  {
    value: 'dateOnboarded',
    text: 'Date Onboarded'
  },
  {
    value: 'accountStatus',
    text: 'Account Status'
  }
];

export const ReportFilterPanel = {
  type: 'panel',
  name: 'reportFilterPanel',
  elements: [
    {
      name: 'reportType',
      type: 'dropdown',
      title: 'Select from the list below:',
      width: '600',
      choices: [
        {
          value: ReportType.UserAccountDetails.toString(),
          text: 'User Accounts Details',
          visibleIf: `{currentUser_isAdministrator}==true
          or {currentUser_isDivisionLearningCoordinatorOrSchoolStaffDeveloper}==true`
        },
        {
          value: ReportType.AccountStatistics.toString(),
          text: 'Account Statistics',
          visibleIf: `{currentUser_isOverallSystemAdministrator}==true
          or {currentUser_isUserAccountAdministrator}==true`
        },
        {
          value: ReportType.PrivilegedAccounts.toString(),
          text: 'Privileged Accounts',
          visibleIf: `{currentUser_isAdministrator}==true
          or {currentUser_isDivisionLearningCoordinatorOrSchoolStaffDeveloper}==true`
        },
        {
          value: ReportType.OfficerPWCMeeting.toString(),
          text: 'Onboarding Statistics',
          visibleIf: `{currentUser_isOverallSystemAdministrator}==true
          or {currentUser_isUserAccountAdministrator}==true`
        },
        {
          value: ReportType.AllUsers.toString(),
          text: 'User Accounts Details (With Welcome Email)',
          visibleIf: `{currentUser_isOverallSystemAdministrator}==true
          or {currentUser_isUserAccountAdministrator}==true`
        },
        {
          value: ReportType.UserAccountStatus.toString(),
          text: 'Account Application Report'
        }
      ]
    },
    {
      name: 'fromDate',
      title: 'Start Date',
      type: 'datepicker',
      inputType: 'date',
      placeHolder: 'dd/mm/yyyy',
      dateFormat: 'dd/mm/yy',
      config: {
        changeMonth: true,
        changeYear: true,
        maxDate: '+0d'
      },
      endDate: '0d',
      width: '300',
      isRequired: true,
      requiredErrorText: 'Start Date is required.',
      visibleIf: `{reportType} notempty
        and [${reportTypeUsingIframe}] notcontains {reportType}`
    },
    {
      name: 'toDate',
      title: 'End Date',
      type: 'datepicker',
      inputType: 'date',
      placeHolder: 'dd/mm/yyyy',
      dateFormat: 'dd/mm/yy',
      config: {
        changeMonth: true,
        changeYear: true,
        maxDate: '+0d'
      },
      startWithNewLine: false,
      width: '300',
      isRequired: true,
      requiredErrorText: 'End Date is required.',
      visibleIf: `{reportType} notempty
        and [${reportTypeUsingIframe}] notcontains {reportType}`,
      validators: [
        {
          type: 'expression',
          text: 'End Date should be after Start Date.',
          expression: '{fromDate} empty or compareDates({toDate},{fromDate})'
        }
      ]
    }
  ]
};

const UserAccountDetailsPanel = {
  type: 'panel',
  name: 'UserAccountDetailsPanel',
  visibleIf: `{reportType} == "${ReportType.UserAccountDetails.toString()}"`,
  elements: []
};

export const AccountStatisticsFields = [
  {
    value: 'accountStatistics.All',
    text: 'Total Created'
  },
  {
    value: 'accountStatistics.New',
    text: 'New'
  },
  {
    value: 'accountStatistics.Active',
    text: 'Active'
  },
  {
    value: 'accountStatistics.Inactive',
    text: 'Suspended'
  },
  {
    value: 'accountStatistics.Deactive',
    text: 'Deleted'
  },
  {
    value: 'accountStatistics.IdentityServerLocked',
    text: 'Locked'
  },
  {
    value: 'accountStatistics.PendingApproval1st',
    text: 'Pending 1st Level Approval'
  },
  {
    value: 'accountStatistics.PendingApproval2nd',
    text: 'Pending 2nd Level Approval'
  },
  {
    value: 'accountStatistics.PendingApproval3rd',
    text: 'Pending Approval from Overall System Administrator'
  },
  {
    value: 'accountStatistics.Rejected',
    text: 'Rejected'
  },
  {
    value: 'accountStatistics.Archived',
    text: 'Archived'
  }
];

export const LoginStatisticsFields = [
  {
    value: 'eventStatistics.LoginSuccess.NumberOfUniqueUsers',
    text: 'Total unique number of users'
  },
  {
    value: 'eventStatistics.LoginSuccess.NumberOfEvents',
    text: 'Total number of logins'
  }
];

export const OnBoardingStatisticsFields = [
  {
    value: 'onBoardingStatistics.NotStarted',
    text: 'Not Started'
  },
  {
    value: 'onBoardingStatistics.Started',
    text: 'Started'
  },
  {
    value: 'onBoardingStatistics.Completed',
    text: 'Completed'
  }
];

const AccountStatisticPanel = {
  type: 'panel',
  name: 'accountStatisticPanel',
  visibleIf: `{reportType} == "${ReportType.AccountStatistics.toString()}"`,
  elements: []
};

export const PrivilegedAccountFields = [
  {
    value: 'typeOfOrganization',
    text: 'Type of Organisation'
  },
  {
    value: 'departmentPathName',
    text: 'Place of Work'
  },
  {
    value: 'fullName',
    text: 'Full Name'
  },
  {
    value: 'designation',
    text: 'Designation'
  },
  {
    value: 'systemRoles',
    text: 'System Roles'
  },
  {
    value: 'emailAddress',
    text: 'Email Address'
  },
  {
    value: 'created',
    text: 'Date of account created'
  },
  {
    value: 'lastLoginDate',
    text: 'Date of last login'
  }
];

const PrivilegedAccountPanel = {
  type: 'panel',
  name: 'privilegedAccountPanel',
  visibleIf: `{reportType} == "${ReportType.PrivilegedAccounts.toString()}"`,
  elements: [
    // TODO: Uncomment this in case we need to show the filter on the Org Units.
    // {
    //   type: 'tagbox',
    //   name: 'departmentId',
    //   title: 'Organisational Unit(s)',
    //   renderAs: 'select2',
    //   requiredErrorText: 'Please fill in the new Organisational Unit(s).',
    //   isRequired: true,
    //   choicesByUrl: {
    //     url: `{organizationApi_BaseUrl}/departments/1/hierarchydepartmentinfos?includeParent=false&includeChildren=true&includeDepartmentType=false&countChildren=false&getParentNode=false&countUser=false`,
    //     valueName: 'identity.id',
    //     titleName: 'name'
    //   },
    //   choicesOrder: 'asc'
    // },
    // {
    //   type: 'checkbox',
    //   name: 'includeSubDepartment',
    //   titleLocation: 'hidden',
    //   choices: [
    //     {
    //       value: 'true',
    //       text: 'Include sub-organisations'
    //     }
    //   ]
    // }
  ]
};

export const ReportFormJSON = {
  elements: [
    ReportFilterPanel,
    UserAccountDetailsPanel,
    AccountStatisticPanel,
    PrivilegedAccountPanel,
    {
      type: 'html',
      name: 'reportIframe',
      visibleIf: `{reportType} notempty and {iframeUrl} notempty and [${reportTypeUsingIframe}] contains {reportType}`,
      html: `
        <iframe id="embededReport" src="{iframeUrl}"
          width="100%" height="100%">
        </iframe>
      `
    }
  ]
};
