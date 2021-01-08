import { AnnouncementStatus } from '@opal20/domain-api';

export const ANNOUNCEMENT_STATUS_COLOR_MAP = {
  [AnnouncementStatus.Scheduled]: {
    text: 'Scheduled',
    color: '#EFDC33'
  },
  [AnnouncementStatus.Cancelled]: {
    text: 'Cancelled',
    color: '#FF6262'
  },
  [AnnouncementStatus.Sent]: {
    text: 'Sent',
    color: '#3BDC87'
  }
};
