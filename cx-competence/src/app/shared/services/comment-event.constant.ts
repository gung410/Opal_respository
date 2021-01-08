export enum CommentEventAction {
  CommentCreated = 'pdpmapp.events.user_has_created_comment',
  CommentUpdated = 'pdpmapp.events.user_has_modified_comment',
  CommentDeleted = 'pdpmapp.events.user_has_deleted_comment',
}

export enum CommentEventEntity {
  IdpLearningNeedsAnalysis = 'idp.needs',
  IdpPlan = 'idp.plans',
  IdpPdo = 'idp.actionitems',
  OdpLearningPlan = 'odp.plans',
  OdpLearningDirection = 'odp.directions',
  OdpKeyLearningProgramme = 'odp.programmes',
  OdpObjectives = 'odp.objectives',
}
