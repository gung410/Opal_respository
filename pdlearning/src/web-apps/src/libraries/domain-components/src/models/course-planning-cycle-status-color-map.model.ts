import { CoursePlanningCycleStatus } from '@opal20/domain-api';
export const COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP = {
  [CoursePlanningCycleStatus.NotStarted]: {
    text: 'Not-Started',
    color: '#EFDC33'
  },
  [CoursePlanningCycleStatus.InProgress]: {
    text: 'In-Progress',
    color: '#EFDC33'
  },
  [CoursePlanningCycleStatus.Completed]: {
    text: 'Completed',
    color: '#3BDC87'
  }
};
