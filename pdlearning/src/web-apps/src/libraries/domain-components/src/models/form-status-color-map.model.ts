import { FormStatus } from '@opal20/domain-api';

export const FORM_STATUS_COLOR_MAP = {
  [FormStatus.Draft]: {
    text: 'Draft',
    color: '#D8DCE6'
  },
  [FormStatus.Published]: {
    text: 'Published',
    color: '#3BDC87'
  },
  [FormStatus.Unpublished]: {
    text: 'Unpublished',
    color: '#EFDC33'
  },
  [FormStatus.PendingApproval]: {
    text: 'Pending Approval',
    color: '#EFDC33'
  },
  [FormStatus.Approved]: {
    text: 'Approved',
    color: '#3BDC87'
  },
  [FormStatus.Rejected]: {
    text: 'Rejected',
    color: '#FF6262'
  },
  [FormStatus.ReadyToUse]: {
    text: 'Ready For Use',
    color: 'rgb(14, 64, 159)'
  },
  [FormStatus.Archived]: {
    text: 'Archived',
    color: '#FF6262'
  }
};
