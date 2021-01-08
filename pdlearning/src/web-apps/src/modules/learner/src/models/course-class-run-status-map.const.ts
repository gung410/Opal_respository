import { MyRegistrationStatus } from '@opal20/domain-api';

export const COURSE_CLASS_RUN_STATUS_COLOR_MAP = {
  ['null']: {
    text: 'Apply',
    color: '#A1A8B6'
  },
  [MyRegistrationStatus.PendingConfirmation]: {
    text: 'Pending Approval',
    color: '#EFDC33'
  },
  [MyRegistrationStatus.Approved]: {
    text: 'Approved',
    color: '#3BDC87'
  },
  [MyRegistrationStatus.Rejected]: {
    text: 'Rejected',
    color: '#FF6262'
  },
  [MyRegistrationStatus.ConfirmedByCA]: {
    text: 'Confirmed',
    color: '#3BDC87'
  },
  [MyRegistrationStatus.RejectedByCA]: {
    text: 'Rejected by Course Admin',
    color: '#FF6262'
  },
  [MyRegistrationStatus.WaitlistPendingApprovalByLearner]: {
    text: 'Waitlist Pending Approval By Learner',
    color: '#EFDC33'
  },
  [MyRegistrationStatus.WaitlistRejected]: {
    text: 'Waitlist Rejected',
    color: '#FF6262'
  },
  [MyRegistrationStatus.WaitlistConfirmed]: {
    text: 'Waitlist Approved',
    color: '#3BDC87'
  },
  [MyRegistrationStatus.OfferPendingApprovalByLearner]: {
    text: 'Offer Pending Confirmation',
    color: '#EFDC33'
  },
  [MyRegistrationStatus.OfferRejected]: {
    text: 'Waitlist Unsuccessful',
    color: '#FF6262'
  },
  [MyRegistrationStatus.OfferConfirmed]: {
    text: 'Confirmed',
    color: '#3BDC87'
  }
};
