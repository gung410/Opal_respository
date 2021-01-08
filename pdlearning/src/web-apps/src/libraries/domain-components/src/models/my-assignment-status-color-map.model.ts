import { MyAssignmentStatus } from '@opal20/domain-api';

export const MY_ASSIGNMENT_STATUS_COLOR_MAP = {
  [MyAssignmentStatus.NotStarted]: {
    text: 'Not Started',
    color: '#D8DCE6'
  },
  [MyAssignmentStatus.InProgress]: {
    text: 'In-Progress',
    color: '#EFDC33'
  },
  [MyAssignmentStatus.Completed]: {
    text: 'Completed',
    color: '#3BDC87'
  },
  [MyAssignmentStatus.LateSubmission]: {
    text: 'Late in Submission',
    color: '#58A8F7'
  },
  [MyAssignmentStatus.Incomplete]: {
    text: 'Incomplete',
    color: '#FF6262'
  }
};
