import {
  ClassRunChangeStatus,
  MyCourseDisplayStatus,
  MyCourseStatus,
  MyDigitalContentStatus,
  MyRegistrationStatus,
  WithdrawalStatus
} from '@opal20/domain-api';

export const WITHDRAWAL_STATUS_HAVE_TO_SHOW = [WithdrawalStatus.PendingConfirmation, WithdrawalStatus.Approved, WithdrawalStatus.Withdrawn];

export const MY_COURSE_STATUS_DISPLAY_MAP: Map<MyCourseStatus, string> = new Map<MyCourseStatus, string>([
  [MyCourseStatus.InProgress, 'InProgress'],
  [MyCourseStatus.Passed, 'InProgress'],
  [MyCourseStatus.Failed, 'InProgress'],
  [MyCourseStatus.Completed, 'Completed']
]);

export const DISPLAY_STATUS_MAP: Map<MyCourseDisplayStatus, string> = new Map<MyCourseDisplayStatus, string>([
  [MyCourseDisplayStatus.PendingConfirmation, 'Registration Pending Approval'],
  [MyCourseDisplayStatus.Approved, 'Registration Pending Confirmation'],
  [MyCourseDisplayStatus.Rejected, 'Registration Unsuccessful'],
  [MyCourseDisplayStatus.ConfirmedByCA, 'Registration Confirmed'],
  [MyCourseDisplayStatus.RejectedByCA, 'Registration Unsuccessful'],
  [MyCourseDisplayStatus.WaitlistPendingApprovalByLearner, 'On Waitlist'],
  [MyCourseDisplayStatus.WaitlistConfirmed, 'On Waitlist'],
  [MyCourseDisplayStatus.WaitlistRejected, 'Registration Unsuccessful'],
  [MyCourseDisplayStatus.OfferPendingApprovalByLearner, 'On Waitlist (Offer)'],
  [MyCourseDisplayStatus.OfferRejected, 'Registration Unsuccessful'],
  [MyCourseDisplayStatus.OfferConfirmed, 'Registration Confirmed'],
  [MyCourseDisplayStatus.Cancelled, 'Registration Unsuccessful'],
  [MyCourseDisplayStatus.Rescheduled, 'Registration Rescheduled'],
  [MyCourseDisplayStatus.Expired, 'Registration Unsuccessful'],

  [MyCourseDisplayStatus.NominatedPendingConfirmation, 'Nomination Pending Approval'],
  [MyCourseDisplayStatus.NominatedApproved, 'Nomination Pending Approval'],
  [MyCourseDisplayStatus.NominatedRejected, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.NominatedConfirmedByCA, 'Nomination Confirmed'],
  [MyCourseDisplayStatus.NominatedRejectedByCA, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.NominatedWaitlistPendingApprovalByLearner, 'On Waitlist'],
  [MyCourseDisplayStatus.NominatedWaitlistConfirmed, 'On Waitlist'],
  [MyCourseDisplayStatus.NominatedWaitlistRejected, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.NominatedOfferPendingApprovalByLearner, 'On Waitlist (Offer)'],
  [MyCourseDisplayStatus.NominatedOfferRejected, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.NominatedOfferConfirmed, 'Nomination Confirmed'],
  [MyCourseDisplayStatus.NominatedCancelled, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.NominatedRescheduled, 'Nomination Rescheduled'],
  [MyCourseDisplayStatus.NominatedExpired, 'Nomination Unsuccessful'],

  [MyCourseDisplayStatus.AddedByCAPendingConfirmation, 'Nomination Pending Approval'],
  [MyCourseDisplayStatus.AddedByCAApproved, 'Nomination Pending Approval'],
  [MyCourseDisplayStatus.AddedByCARejected, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.AddedByCAConfirmedByCA, 'Nomination Confirmed'],
  [MyCourseDisplayStatus.AddedByCARejectedByCA, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.AddedByCAWaitlistPendingApprovalByLearner, 'On Waitlist'],
  [MyCourseDisplayStatus.AddedByCAWaitlistConfirmed, 'On Waitlist'],
  [MyCourseDisplayStatus.AddedByCAWaitlistRejected, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.AddedByCAOfferPendingApprovalByLearner, 'On Waitlist (Offer)'],
  [MyCourseDisplayStatus.AddedByCAOfferRejected, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.AddedByCAOfferConfirmed, 'Nomination Confirmed'],
  [MyCourseDisplayStatus.AddedByCACancelled, 'Nomination Unsuccessful'],
  [MyCourseDisplayStatus.AddedByCARescheduled, 'Nomination Rescheduled'],
  [MyCourseDisplayStatus.AddedByCAExpired, 'Nomination Unsuccessful'],

  [MyCourseDisplayStatus.WithdrawalPendingConfirmation, 'Withdrawal Pending Approval'],
  [MyCourseDisplayStatus.WithdrawalApproved, 'Withdrawal Pending Confirmation'],
  [MyCourseDisplayStatus.WithdrawalRejected, 'Withdrawal Unsuccessful'],
  [MyCourseDisplayStatus.WithdrawalWithdrawn, 'Withdrawal Successful'],
  [MyCourseDisplayStatus.WithdrawalRejectedByCA, 'Withdrawal Unsuccessful'],

  [MyCourseDisplayStatus.ClassRunChangeApproved, 'Change of Class Pending Confirmation'],
  [MyCourseDisplayStatus.ClassRunChangePendingConfirmation, 'Change of Class Pending Approval'],
  [MyCourseDisplayStatus.ClassRunChangeRejected, 'Change of Class Unsuccessful'],
  [MyCourseDisplayStatus.ClassRunChangeRejectedByCA, 'Change of Class Unsuccessful'],
  [MyCourseDisplayStatus.ClassRunChangeConfirmedByCA, 'Change of Class Confirmed']
]);

export const WITHDRAWAL_STATUS_TO_DISPLAY_STATUS_MAP: Map<WithdrawalStatus, MyCourseDisplayStatus> = new Map<
  WithdrawalStatus,
  MyCourseDisplayStatus
>([
  [WithdrawalStatus.Approved, MyCourseDisplayStatus.WithdrawalApproved],
  [WithdrawalStatus.PendingConfirmation, MyCourseDisplayStatus.WithdrawalPendingConfirmation],
  [WithdrawalStatus.Rejected, MyCourseDisplayStatus.WithdrawalRejected],
  [WithdrawalStatus.RejectedByCA, MyCourseDisplayStatus.WithdrawalRejectedByCA],
  [WithdrawalStatus.Withdrawn, MyCourseDisplayStatus.WithdrawalWithdrawn]
]);

export const CLASSRUNCHANGE_STATUS_TO_DISPLAY_STATUS_MAP: Map<ClassRunChangeStatus, MyCourseDisplayStatus> = new Map<
  ClassRunChangeStatus,
  MyCourseDisplayStatus
>([
  [ClassRunChangeStatus.Approved, MyCourseDisplayStatus.ClassRunChangeApproved],
  [ClassRunChangeStatus.PendingConfirmation, MyCourseDisplayStatus.ClassRunChangePendingConfirmation],
  [ClassRunChangeStatus.Rejected, MyCourseDisplayStatus.ClassRunChangeRejected],
  [ClassRunChangeStatus.RejectedByCA, MyCourseDisplayStatus.ClassRunChangeRejectedByCA],
  [ClassRunChangeStatus.ConfirmedByCA, MyCourseDisplayStatus.ClassRunChangeConfirmedByCA]
]);

export const REJECTED_REGISTRATION_STATUSES = [
  MyRegistrationStatus.Rejected,
  MyRegistrationStatus.OfferRejected,
  MyRegistrationStatus.WaitlistRejected,
  MyRegistrationStatus.RejectedByCA
];

export const APPLIED_REGISTRATION_STATUSES = [
  MyRegistrationStatus.PendingConfirmation,
  MyRegistrationStatus.Approved,
  MyRegistrationStatus.ConfirmedByCA,
  MyRegistrationStatus.WaitlistPendingApprovalByLearner,
  MyRegistrationStatus.WaitlistConfirmed,
  MyRegistrationStatus.OfferPendingApprovalByLearner,
  MyRegistrationStatus.OfferConfirmed
];

export const MY_DIGITAL_CONTENT_STATUS_DISPLAY_MAP: Map<MyDigitalContentStatus, string> = new Map<MyDigitalContentStatus, string>([
  [MyDigitalContentStatus.InProgress, 'InProgress'],
  [MyDigitalContentStatus.Completed, 'Completed']
]);
