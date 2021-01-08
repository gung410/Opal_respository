import { CourseStatus } from '@opal20/domain-api';

export const COURSE_STATUS_COLOR_MAP = {
  [CourseStatus.Draft]: {
    text: CourseStatus.Draft,
    color: '#D8DCE6'
  },
  [CourseStatus.Published]: {
    text: CourseStatus.Published,
    color: '#3BDC87'
  },
  [CourseStatus.Unpublished]: {
    text: CourseStatus.Unpublished,
    color: '#EFDC33'
  },
  [CourseStatus.PendingApproval]: {
    text: 'Pending Approval',
    color: '#EFDC33'
  },
  [CourseStatus.Approved]: {
    text: CourseStatus.Approved,
    color: '#3BDC87'
  },
  [CourseStatus.Rejected]: {
    text: CourseStatus.Rejected,
    color: '#FF6262'
  },
  [CourseStatus.Completed]: {
    text: CourseStatus.Completed,
    color: '#3BDC87'
  },
  [CourseStatus.Archived]: {
    text: CourseStatus.Archived,
    color: '#FF6262'
  }
};

export const COURSE_IN_COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP = {
  [CourseStatus.Draft]: {
    text: CourseStatus.Draft,
    color: '#D8DCE6'
  },
  [CourseStatus.Published]: {
    text: CourseStatus.Published,
    color: '#3BDC87'
  },
  [CourseStatus.Unpublished]: {
    text: CourseStatus.Unpublished,
    color: '#EFDC33'
  },
  [CourseStatus.PendingApproval]: {
    text: 'Pending Approval',
    color: '#EFDC33'
  },
  [CourseStatus.Approved]: {
    text: 'Pending Confirmation',
    color: '#3BDC87'
  },
  [CourseStatus.Rejected]: {
    text: CourseStatus.Rejected,
    color: '#FF6262'
  },
  [CourseStatus.PlanningCycleVerified]: {
    text: 'Confirmed',
    color: '#3BDC87'
  },
  [CourseStatus.PlanningCycleCompleted]: {
    text: 'Completed Planning',
    color: '#3BDC87'
  },
  [CourseStatus.VerificationRejected]: {
    text: 'Verification Rejected',
    color: '#FF6262'
  },
  [CourseStatus.Completed]: {
    text: CourseStatus.Completed,
    color: '#3BDC87'
  },
  [CourseStatus.Archived]: {
    text: CourseStatus.Archived,
    color: '#FF6262'
  }
};
