import { AttendanceStatus } from '@opal20/domain-api';

export const ATTENDANCE_TRACKING_STATUS_COLOR_MAP = {
  [AttendanceStatus.Present]: {
    text: AttendanceStatus.Present,
    color: '#3BDC87'
  },
  [AttendanceStatus.Absent]: {
    text: AttendanceStatus.Absent,
    color: '#FF6262'
  }
};
