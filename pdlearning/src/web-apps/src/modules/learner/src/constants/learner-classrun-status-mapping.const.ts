import { ClassRunRescheduleStatus, ClassRunStatus } from '@opal20/domain-api';

export const LEARNER_CLASSRUN_STATUS_COLOR_MAP = {
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
  [ClassRunRescheduleStatus.Approved]: {
    text: 'Rescheduled',
    color: '#3BDC87'
  }
};
