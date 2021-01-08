export interface IMyCourseModel {
  id: string;
  courseId: string;
  userId: string;
  version?: string | undefined;
  status: MyCourseStatus;
  reviewStatus?: string | undefined;
  progressMeasure?: number | undefined;
  lastLogin?: Date | undefined;
  disenrollUtc?: Date | undefined;
  readDate?: Date | undefined;
  reminderSentDate?: Date | undefined;
  startDate?: Date | undefined;
  endDate?: Date | undefined;
  completedDate?: Date | undefined;
  createdDate: Date;
  createdBy: string;
  changedDate?: Date | undefined;
  changedBy?: string | undefined;
  currentLecture?: string;
  displayStatus?: MyCourseDisplayStatus;
  hasContentChanged?: boolean;
}

export class MyCourseModel implements IMyCourseModel {
  public id: string;
  public courseId: string;
  public userId: string;
  public version: string;
  public status: MyCourseStatus;
  public reviewStatus: string;
  public progressMeasure: number;
  public lastLogin: Date;
  public disenrollUtc: Date;
  public readDate: Date;
  public reminderSentDate: Date;
  public startDate: Date;
  public endDate: Date;
  public completedDate: Date;
  public createdDate: Date;
  public createdBy: string;
  public changedDate: Date;
  public changedBy: string;
  public currentLecture?: string;

  public displayStatus?: MyCourseDisplayStatus;

  public hasContentChanged?: boolean;
  constructor(data?: IMyCourseModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.courseId = data.courseId;
    this.userId = data.userId;
    this.version = data.version;
    this.status = data.status;
    this.reviewStatus = data.reviewStatus;
    this.progressMeasure = data.progressMeasure;
    this.lastLogin = data.lastLogin;
    this.disenrollUtc = data.disenrollUtc;
    this.readDate = data.readDate;
    this.reminderSentDate = data.reminderSentDate;
    this.startDate = data.startDate;
    this.endDate = data.endDate;
    this.completedDate = data.completedDate !== undefined ? new Date(data.completedDate) : undefined;
    this.createdDate = new Date(data.createdDate);
    this.createdBy = data.createdBy;
    this.changedDate = data.changedDate !== undefined ? new Date(data.changedDate) : undefined;
    this.changedBy = data.changedBy;
    this.currentLecture = data.currentLecture;
    this.displayStatus = data.displayStatus;
    this.hasContentChanged = data.hasContentChanged;
  }
}

export enum MyCourseStatus {
  InProgress = 'InProgress',
  Completed = 'Completed',
  NotStarted = 'NotStarted',
  Passed = 'Passed',
  Failed = 'Failed'
}

export enum MyRegistrationStatus {
  PendingConfirmation = 'PendingConfirmation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  ConfirmedByCA = 'ConfirmedByCA',
  RejectedByCA = 'RejectedByCA',
  WaitlistPendingApprovalByLearner = 'WaitlistPendingApprovalByLearner',
  WaitlistConfirmed = 'WaitlistConfirmed',
  WaitlistRejected = 'WaitlistRejected',
  OfferPendingApprovalByLearner = 'OfferPendingApprovalByLearner',
  OfferRejected = 'OfferRejected',
  OfferConfirmed = 'OfferConfirmed'
}

export enum MyCourseDisplayStatus {
  WithdrawalPendingConfirmation = 'WithdrawalPendingConfirmation',
  WithdrawalRejected = 'WithdrawalRejected',
  WithdrawalApproved = 'WithdrawalApproved',
  WithdrawalWithdrawn = 'WithdrawalWithdrawn',
  WithdrawalRejectedByCA = 'WithdrawalRejectedByCA',

  ClassRunChangePendingConfirmation = 'ClassRunChangePendingConfirmation',
  ClassRunChangeApproved = 'ClassRunChangeApproved',
  ClassRunChangeRejected = 'ClassRunChangeRejected',
  ClassRunChangeConfirmedByCA = 'ClassRunChangeConfirmedByCA',
  ClassRunChangeRejectedByCA = 'ClassRunChangeRejectedByCA',

  PendingConfirmation = 'PendingConfirmation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  ConfirmedByCA = 'ConfirmedByCA',
  RejectedByCA = 'RejectedByCA',
  WaitlistPendingApprovalByLearner = 'WaitlistPendingApprovalByLearner',
  WaitlistConfirmed = 'WaitlistConfirmed',
  WaitlistRejected = 'WaitlistRejected',
  OfferPendingApprovalByLearner = 'OfferPendingApprovalByLearner',
  OfferRejected = 'OfferRejected',
  OfferConfirmed = 'OfferConfirmed',
  Expired = 'Expired',

  Cancelled = 'Cancelled',
  Rescheduled = 'Rescheduled',

  NominatedPendingConfirmation = 'NominatedPendingConfirmation',
  NominatedApproved = 'NominatedApproved',
  NominatedRejected = 'NominatedRejected',
  NominatedConfirmedByCA = 'NominatedConfirmedByCA',
  NominatedRejectedByCA = 'NominatedRejectedByCA',
  NominatedWaitlistPendingApprovalByLearner = 'NominatedWaitlistPendingApprovalByLearner',
  NominatedWaitlistConfirmed = 'NominatedWaitlistConfirmed',
  NominatedWaitlistRejected = 'NominatedWaitlistRejected',
  NominatedOfferPendingApprovalByLearner = 'NominatedOfferPendingApprovalByLearner',
  NominatedOfferRejected = 'NominatedOfferRejected',
  NominatedOfferConfirmed = 'NominatedOfferConfirmed',
  NominatedExpired = 'NominatedExpired',

  NominatedCancelled = 'NominatedCancelled',
  NominatedRescheduled = 'NominatedRescheduled',

  AddedByCAPendingConfirmation = 'AddedByCAPendingConfirmation',
  AddedByCAApproved = 'AddedByCAApproved',
  AddedByCARejected = 'AddedByCARejected',
  AddedByCAConfirmedByCA = 'AddedByCAConfirmedByCA',
  AddedByCARejectedByCA = 'AddedByCARejectedByCA',
  AddedByCAWaitlistPendingApprovalByLearner = 'AddedByCAWaitlistPendingApprovalByLearner',
  AddedByCAWaitlistConfirmed = 'AddedByCAWaitlistConfirmed',
  AddedByCAWaitlistRejected = 'AddedByCAWaitlistRejected',
  AddedByCAOfferPendingApprovalByLearner = 'AddedByCAOfferPendingApprovalByLearner',
  AddedByCAOfferRejected = 'AddedByCAOfferRejected',
  AddedByCAOfferConfirmed = 'AddedByCAOfferConfirmed',
  AddedByCAExpired = 'AddedByCAExpired',

  AddedByCACancelled = 'AddedByCACancelled',
  AddedByCARescheduled = 'AddedByCARescheduled'
}
