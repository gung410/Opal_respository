export enum ApprovalTargetEnum {
  SelfAssignPDO = 'self-assign-pdo',
  LearningPlan = 'learning-plan',
  LearningDirection = 'learning-direction',
  LNA = 'lna',
  PDPlan = 'pdplan',
  ClassRegistration = 'class-registration',
  ClassWidthdrawal = 'class-withdrawal',
  ClassChangeRequest = 'class-change-request',
  Nominations = 'nomination',
  NominationsLearner = 'nomination-learner',
  NominationsGroup = 'nomination-group',
  NominationDepartment = 'nomination-department',
  MassNomination = 'mass-nomination',
  AdhocNominations = 'adhoc-nomination',
  AdhocNominationsLearner = 'adhoc-learner',
  AdhocNominationsGroup = 'adhoc-group',
  AdhocNominationDepartment = 'adhoc-department',
  AdhocMassNomination = 'adhoc-mass-nomination',
}
export enum ChangeNominationStatusTargetEnum {
  Learner = 'Learner',
  Group = 'Group',
  Department = 'Department',
  MassNomination = 'MassNomination',
  AdHocMassNomination = 'AdHocMassNomination',
}

export enum ApprovalTypeEnum {
  Admin = 'Admin',
  AO = 'AO',
}
