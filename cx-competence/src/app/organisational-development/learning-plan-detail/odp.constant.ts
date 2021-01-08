export enum OdpActivity {
  Plan = 'LearningPlan',
  Direction = 'LearningDirection',
  Programme = 'LearningProgramme',
}

export enum OdpActivityName {
  LearningPlan = 'Organisational PD Journey',
  LearningDirection = 'Learning Direction',
  LearningProgramme = 'Key Learning Programme',
}

export enum OdpStatusEnum {
  NotStarted = 1,
  Started = 2,
  PendingForApproval = 3,
  Approved = 4,
  Rejected = 5,
  Completed = 6,
}

export enum OdpStatusCode {
  NotStarted = 'NotStarted',
  Started = 'Started',
  PendingForApproval = 'PendingForApproval',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Completed = 'Completed',
}

export enum NominateStatusEnum {
  PendingForApproval = 3,
  Approved = 4,
  Rejected = 5,
  PendingForApproval2nd = 13,
  Rejected2nd = 14,
}

export enum NominateStatusCodeEnum {
  PendingForApproval = 'PendingForApproval',
  PendingForApproval2nd = 'PendingForApproval2nd',
  PendingForApproval3rd = 'PendingForApproval3rd',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Rejected2nd = 'Rejected2nd',
  Rejected3nd = 'Rejected3nd',
  Rejected4th = 'Rejected4th',
  Rejected5th = 'Rejected5th',
  NotNominated = 'NotNominated',
}

export enum OdpActionEnum {
  Submit = 1,
  Approve = 2,
  Reject = 3,
}

export class NodeStatus {
  nodeShortName: string;
  nodeStatus: NodeStatusType;
}

export enum NodeStatusType {
  Draft = 'draft',
  Pending = 'pending',
  Approved = 'approved',
  Rejected = 'rejected',
}

export enum NominationChangeStatusAction {
  Approve = 'Approve',
  Reject = 'Reject',
}
export enum NominationSource {
  OPJ = 'nominations',
  Adhoc = 'adhocnominations',
}
