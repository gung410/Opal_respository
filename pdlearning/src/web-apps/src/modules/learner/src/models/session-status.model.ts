export enum AttendanceStatusEnum {
  Completed = 'Completed',
  Incompleted = 'Incompleted'
}
export const ATTENDANCE_STATUS_COLOR_MAP = {
  [AttendanceStatusEnum.Completed]: {
    text: 'Completed',
    color: '#3BDC87'
  },
  [AttendanceStatusEnum.Incompleted]: {
    text: 'Incomplete',
    color: '#FF6262'
  }
};
