import { BlockoutDateStatus } from '@opal20/domain-api';

export const BLOCKOUT_DATE_STATUS_COLOR_MAP = {
  [BlockoutDateStatus.Draft]: {
    text: BlockoutDateStatus.Draft,
    color: '#D8DCE6'
  },
  [BlockoutDateStatus.Active]: {
    text: BlockoutDateStatus.Active,
    color: '#3BDC87'
  }
};
