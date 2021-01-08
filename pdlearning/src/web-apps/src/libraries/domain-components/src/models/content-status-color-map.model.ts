import { ContentStatus } from '@opal20/domain-api';

export const CONTENT_STATUS_COLOR_MAP = {
  [ContentStatus.Draft]: {
    text: ContentStatus.Draft,
    color: '#D8DCE6'
  },
  [ContentStatus.Expired]: {
    text: ContentStatus.Expired,
    color: '#FF6262'
  },
  [ContentStatus.Published]: {
    text: ContentStatus.Published,
    color: '#3BDC87'
  },
  [ContentStatus.Unpublished]: {
    text: ContentStatus.Unpublished,
    color: '#EFDC33'
  },
  [ContentStatus.PendingApproval]: {
    text: 'Pending Approval',
    color: '#EFDC33'
  },
  [ContentStatus.Approved]: {
    text: ContentStatus.Approved,
    color: '#3BDC87'
  },
  [ContentStatus.Rejected]: {
    text: ContentStatus.Rejected,
    color: '#FF6262'
  }
};
