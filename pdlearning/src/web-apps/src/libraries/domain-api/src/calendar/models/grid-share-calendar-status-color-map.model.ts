import { ShareCalendarActionsEnum } from '../enums/share-calendar-actions-enum';

export const GRID_SHARE_CALENDAR_STATUS_COLOR_MAP = {
  [ShareCalendarActionsEnum.Share]: {
    text: 'Yes',
    color: '#3BDC87'
  },
  [ShareCalendarActionsEnum.Unshare]: {
    text: 'No',
    color: '#D8DCE6'
  }
};
