export enum IdpActivityEnum {
  LearningNeed = 'LearningNeed',
  LearningPlan = 'LearningPlan',
}

export enum IdpStatusEnum {
  NotStarted = 1,
  Started = 2,
  PendingForApproval = 3,
  PendingForApproval2nd = 14,
  Approved = 4,
  Rejected = 5,
  Completed = 6,
  NotAdded = 0,
  ExternalPendingForApproval = 18,
  ExternalRejected = 19,
  InCompleted = 99999, //TODO: need to insert to statusType in DB if needed in future, currently this status only use in UI to represent the status
}

export enum IdpStatusCodeEnum {
  NotStarted = 'NotStarted',
  Started = 'Started',
  PendingForApproval = 'PendingForApproval',
  PendingForApproval2nd = 'PendingForApproval2nd',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Completed = 'Completed',
  NotAdded = 'NotAdded',
  ExternalPendingForApproval = 'ExternalPendingForApproval',
  ExternalRejected = 'ExternalRejected',
  InCompleted = 'InCompleted', //TODO: need to insert to statusType in DB if needed in future, currently this status only use in UI to represent the status
}

export enum IdpActionEnum {
  Submit = 1,
  Approve = 2,
  Reject = 3,
}

export enum IDPMode {
  Normal = 'normal',
  ReportingOfficer = 'reportingofficer',
  Learner = 'learner',
}

export enum PDEvaluationType {
  Approve = 'approve',
  Reject = 'reject',
}

export const EvaluationTypeToIdpStatusCode = {
  [PDEvaluationType.Approve]: IdpStatusCodeEnum.Approved,
  [PDEvaluationType.Reject]: IdpStatusCodeEnum.Rejected,
};

export enum IDPTabsMenuEnum {
  LearningNeedAnalysis = 'learning-need-analysis-tab',
  LearningNeed = 'learning-need-tab',
  PDPlan = 'pd-plan-tab',
  EPortfolio = 'e-portfolio-tab',
}
