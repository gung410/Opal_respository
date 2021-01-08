import { ClassRunCancellationStatus, ClassRunRescheduleStatus, ClassRunStatus } from '@opal20/domain-api';

export const CLASSRUN_STATUS_COLOR_MAP = {
  [ClassRunStatus.Published]: {
    text: ClassRunStatus.Published,
    color: '#3BDC87'
  },
  [ClassRunStatus.Unpublished]: {
    text: ClassRunStatus.Unpublished,
    color: '#EFDC33'
  },
  [ClassRunStatus.Cancelled]: {
    text: ClassRunStatus.Cancelled,
    color: '#FF6262'
  },
  [ClassRunRescheduleStatus.PendingApproval]: {
    text: 'Rescheduling - Pending Approval',
    color: '#EFDC33'
  },
  [ClassRunRescheduleStatus.Approved]: {
    text: ClassRunRescheduleStatus.Approved,
    color: '#3BDC87'
  },
  [ClassRunRescheduleStatus.Rejected]: {
    text: ClassRunRescheduleStatus.Rejected,
    color: '#FF6262'
  },
  [ClassRunCancellationStatus.PendingApproval]: {
    text: 'Cancelling - Pending Approval',
    color: '#EFDC33'
  },
  [ClassRunCancellationStatus.Approved]: {
    text: ClassRunRescheduleStatus.Approved,
    color: '#3BDC87'
  },
  [ClassRunCancellationStatus.Rejected]: {
    text: ClassRunRescheduleStatus.Rejected,
    color: '#FF6262'
  }
};

export const CLASSRUN_RESCHEDULE_STATUS_COLOR_MAP = {
  [ClassRunRescheduleStatus.PendingApproval]: {
    text: 'Rescheduling - Pending Approval',
    color: '#EFDC33'
  },
  [ClassRunRescheduleStatus.Approved]: {
    text: 'Rescheduled',
    color: '#3BDC87'
  },
  [ClassRunRescheduleStatus.Rejected]: {
    text: 'Rescheduling - Rejected',
    color: '#FF6262'
  }
};

export const CLASSRUN_CANCELLATION_STATUS_COLOR_MAP = {
  [ClassRunCancellationStatus.PendingApproval]: {
    text: 'Cancelling - Pending Approval',
    color: '#EFDC33'
  },
  [ClassRunCancellationStatus.Approved]: {
    text: 'Cancelled',
    color: '#3BDC87'
  },
  [ClassRunCancellationStatus.Rejected]: {
    text: 'Cancelling - Rejected',
    color: '#FF6262'
  }
};
