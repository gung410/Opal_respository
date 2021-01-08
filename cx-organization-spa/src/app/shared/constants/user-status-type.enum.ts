// tslint:disable:variable-name

export const StatusTypeEnum = {
  Active: { code: 'Active', text: 'Active' },
  Inactive: { code: 'Inactive', text: 'Suspended' },
  PendingApproval1st: {
    code: 'PendingApproval1st',
    text: 'Pending 1st Level Approval'
  },
  PendingApproval2nd: {
    code: 'PendingApproval2nd',
    text: 'Pending 2nd Level Approval'
  },
  PendingApproval3rd: {
    code: 'PendingApproval3rd',
    text: 'Pending Special Approval'
  },
  Deactive: { code: 'Deactive', text: 'Deleted' },
  Rejected: { code: 'Rejected', text: 'Rejected' },
  IdentityServerLocked: { code: 'IdentityServerLocked', text: 'Locked' },
  Suspended: { code: 'Suspended', text: 'Suspended' },
  Archived: { code: 'Archived', text: 'Archived' },
  Deleted: { code: 'Deleted', text: 'Deleted' },
  New: { code: 'New', text: 'New' },
  All: { code: 'All', text: 'All' },
  Unknown: { code: 'Unknown', text: 'Unknown' },
  CreateUser: { code: 'CreateUser', text: 'CreateUser' }
};

export enum StatusTypeNumberEnum {
  Active = 1,
  Inactive = 2,
  Deactive = 3,
  Pending = 4,
  PendingApproval1st = 5,
  PendingApproval2nd = 6,
  PendingApproval3rd = 7,
  New = 8,
  IdentityServerLocked = 9,
  Hidden = 10,
  Rejected = 11
}

export const StatusReasonTypeConstant = {
  AutomaticallySetDeactive: {
    code: 'Deactive_AutomaticallySetDeactive',
    text: '(Set automatically)'
  },
  NotFoundInSource: {
    code: 'Deactive_NotFoundInSource',
    text: 'Not found in external system'
  },
  ManuallyRejectedPending1st: {
    code: 'Deactive_ManuallyRejected_1stLevel',
    text: 'Rejected at 1st level'
  },
  ManuallyRejectedPending2nd: {
    code: 'Deactive_ManuallyRejected_2ndLevel',
    text: 'Rejected at 2nd level'
  },
  ManuallyRejectedPending3rd: {
    code: 'Deactive_ManuallyRejected_3rdLevel',
    text: 'Rejected at 3rd level'
  },
  ExternallyRejected: {
    code: 'Deactive_SynchronizedFromSource_Rejected',
    text: 'Rejected by external system (Synchronized)'
  },
  ManuallyArchived: {
    code: 'Deactive_ManuallyArchived',
    text: 'Archived (Set manually)'
  },
  AutomaticallyArchived: {
    code: 'Deactive_AutomaticallyArchived',
    text: 'Archived (Set automatically)'
  },
  ExternallyArchived: {
    code: 'Deactive_SynchronizedFromSource_Archived',
    text: 'Archived by external system (Synchronized)'
  },
  ExternallySynchronized: {
    code: 'Deactive_SynchronizedFromSource',
    text: 'Synchronized by external system'
  },
  ManuallySetDeactive: {
    code: 'Deactive_ManuallySetDeactive',
    text: 'Deactive (Set manually)'
  },
  Archived_ManuallyArchived: {
    code: 'Archived_ManuallyArchived',
    text: 'Archived (Set manually)'
  },
  AutomaticallyInactiveAbsenceMoreThan90Days: {
    code: 'Inactive_Automatically_Inactivity',
    text: 'Due to inactivity'
  },
  SynchronizedInactiveAbsence: {
    code: 'Inactive_SynchronizedFromSource_Absence',
    text: 'Long term absence (Set automatically)'
  },
  ManuallyInactiveAbsenceMoreThan90Days: {
    code: 'Inactive_Manually_Absence',
    text: 'Long term absence (Set manually)'
  },
  ManuallyInactiveRetirement: {
    code: 'Inactive_Manually_Retirement',
    text: 'Due to retirement'
  },
  ManuallyInactiveResignation: {
    code: 'Inactive_Manually_Resignation',
    text: 'Due to resignation'
  },
  ManuallyInactiveTermination: {
    code: 'Inactive_Manually_Termination',
    text: 'Due to termination'
  },
  ManuallyInactiveLeftWithoutAdvanceNotice: {
    code: 'Inactive_Manually_LeftWithoutNotice',
    text: 'Left without notice'
  },
  SynchronizedInactiveRetirement: {
    code: 'Inactive_SynchronizedFromSource_Retirement',
    text: 'Due to retirement (Set automatically)'
  },
  SynchronizedInactiveResignation: {
    code: 'Inactive_SynchronizedFromSource_Resignation',
    text: 'Due to resignation (Set automatically)'
  },
  SynchronizedInactiveTermination: {
    code: 'Inactive_SynchronizedFromSource_Resignation',
    text: 'Due to termination (Set automatically)'
  },
  SynchronizedInactiveLeftWithoutAdvanceNotice: {
    code: 'Inactive_SynchronizedFromSource_LeftWithoutAdvanceNotice',
    text: 'Left without notice (Set automatically)'
  }
};
