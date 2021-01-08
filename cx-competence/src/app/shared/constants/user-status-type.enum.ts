export const StatusTypeEnum = {
  Active: { code: 'Active', text: 'Active' },
  Inactive: { code: 'Inactive', text: 'Suspended' },
  PendingApproval1st: {
    code: 'PendingApproval1st',
    text: 'Pending 1st level approval',
  },
  PendingApproval2nd: {
    code: 'PendingApproval2nd',
    text: 'Pending 2nd level approval',
  },
  PendingApproval3rd: {
    code: 'PendingApproval3rd',
    text: 'Pending Approval',
  },
  Deactive: { code: 'Deactive', text: 'Deleted' },
  IdentityServerLocked: { code: 'IdentityServerLocked', text: 'Locked' },
  Suspended: { code: 'Suspended', text: 'Suspended' },
  Deleted: { code: 'Deleted', text: 'Deleted' },
  New: { code: 'New', text: 'New' },
  All: { code: 'All', text: 'All' },
  Archived: { code: 'Archived', text: 'Archived' },
};

export enum StatusTypeNumberEnum {
  Active = 1,
  Inactive,
  Deactive,
  Pending,
  PendingApproval1st,
  PendingApproval2nd,
  PendingApproval3rd,
  New,
}

export enum UserStatusTypeEnum {
  Active = 'Active',
  New = 'New',
  Deactive = 'Deactive',
  Inactive = 'Inactive',
  IdentityServerLocked = 'IdentityServerLocked',
  Archived = 'Archived',
}
