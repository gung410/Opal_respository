import { DigitalContentStatus } from '@opal20/domain-api';

export const DIGITAL_CONTENT_STATUS_MAPPING = {
  [DigitalContentStatus.Draft]: {
    text: DigitalContentStatus.Draft,
    color: '#A1A8B6'
  },
  [DigitalContentStatus.Expired]: {
    text: DigitalContentStatus.Expired,
    color: '#FFB4B4'
  },
  [DigitalContentStatus.Published]: {
    text: DigitalContentStatus.Published,
    color: 'rgb(59, 220, 135)'
  },
  [DigitalContentStatus.Unpublished]: {
    text: DigitalContentStatus.Unpublished,
    color: '#EFDC33'
  },
  [DigitalContentStatus.PendingForApproval]: {
    text: 'Pending Approval',
    color: 'rgb(239, 220, 51)'
  },
  [DigitalContentStatus.Approved]: {
    text: DigitalContentStatus.Approved,
    color: '#3BDC87'
  },
  [DigitalContentStatus.Rejected]: {
    text: DigitalContentStatus.Rejected,
    color: '#FF6262'
  },
  [DigitalContentStatus.ReadyToUse]: {
    text: 'Ready For Use',
    color: '#0E409F'
  },
  [DigitalContentStatus.Archived]: {
    text: 'Archived',
    color: '#FF6262'
  }
};
