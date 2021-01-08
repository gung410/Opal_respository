import { RegistrationLearningStatus, RegistrationStatus, WithdrawalStatus } from '@opal20/domain-api';

export const REGISTRATION_STATUS_COLOR_MAP = {
  [RegistrationStatus.PendingConfirmation]: {
    text: 'Unsuccessful',
    color: '#FF6262'
  },
  [RegistrationStatus.Approved]: {
    text: 'Pending Confirmation',
    color: '#EFDC33'
  },
  [RegistrationStatus.Rejected]: {
    text: 'Unsuccessful',
    color: '#FF6262'
  },
  [RegistrationStatus.ConfirmedByCA]: {
    text: 'Confirmed',
    color: '#3BDC87'
  },
  [RegistrationStatus.RejectedByCA]: {
    text: 'Unsuccessful',
    color: '#FF6262'
  },
  [RegistrationStatus.WaitlistPendingApprovalByLearner]: {
    text: 'On Waitlist',
    color: '#58A8F7'
  },
  [RegistrationStatus.WaitlistConfirmed]: {
    text: 'On Waitlist',
    color: '#58A8F7'
  },
  [RegistrationStatus.WaitlistRejected]: {
    text: 'Unsuccessful',
    color: '#FF6262'
  },
  [RegistrationStatus.OfferPendingApprovalByLearner]: {
    text: 'On Waitlist (Offer)',
    color: '#58A8F7'
  },
  [RegistrationStatus.OfferRejected]: {
    text: 'Unsuccessful',
    color: '#FF6262'
  },
  [RegistrationStatus.OfferConfirmed]: {
    text: 'Confirmed',
    color: '#3BDC87'
  },
  [RegistrationStatus.AddedByCAClassfull]: {
    text: 'On Waitlist',
    color: '#58A8F7'
  },
  [RegistrationStatus.Expired]: {
    text: 'Unsuccessful',
    color: '#FF6262'
  }
};
export const REGISTRATION_CHANGE_CLASSRUN_STATUS_COLOR_MAP = {
  [RegistrationStatus.Approved]: {
    text: 'Change of Class Pending Confirmation',
    color: '#EFDC33'
  },
  [RegistrationStatus.ConfirmedByCA]: {
    text: 'Change of Class Confirmed',
    color: '#3BDC87'
  },
  [RegistrationStatus.RejectedByCA]: {
    text: 'Change of Class Unsuccessful',
    color: '#FF6262'
  }
};

export const REGISTRATION_STATUS_ADDING_PARTICIPANTS_COLOR_MAP = {
  [RegistrationStatus.ConfirmedByCA]: {
    text: 'Added Successfully',
    color: '#3BDC87'
  },
  [RegistrationStatus.AddedByCAConflict]: {
    text: 'Unsuccessful due to Conflict',
    color: '#FF6262'
  },
  [RegistrationStatus.AddedByCAClassfull]: {
    text: 'Unsuccessful - Class Full ',
    color: '#FF6262'
  }
};

export const WITHDRAWAL_STATUS_COLOR_MAP = {
  [WithdrawalStatus.Withdrawn]: {
    text: 'Withdrawal Successful',
    color: '#3BDC87'
  },
  [WithdrawalStatus.RejectedByCA]: {
    text: 'Withdrawal Unsuccessful',
    color: '#FF6262'
  },
  [WithdrawalStatus.Approved]: {
    text: 'Withdrawal Pending Confirmation',
    color: '#EFDC33'
  }
};

export const REGISTRATION_LEARNING_STATUS_COLOR_MAP = {
  [RegistrationLearningStatus.NotStarted]: {
    text: 'Not Started',
    color: '#D8DCE6'
  },
  [RegistrationLearningStatus.InProgress]: {
    text: 'In Progress',
    color: '#EFDC33'
  },
  [RegistrationLearningStatus.Passed]: {
    text: 'In Progress',
    color: '#EFDC33'
  },
  [RegistrationLearningStatus.Failed]: {
    text: 'Incomplete',
    color: '#FF6262'
  },
  [RegistrationLearningStatus.Completed]: {
    text: 'Completed',
    color: '#3BDC87'
  }
};
