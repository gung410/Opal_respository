import { User } from 'app-models/auth.model';
import { BaseUserPermission } from './permission-setting';

export interface ILearningPlanPermission {
  learningPlanPermission: LearningPlanPermission;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initLearningPlanPermissionn(loginUser: User): void;
}

export class LearningPlanPermission extends BaseUserPermission {
  allowViewOverall: boolean;
  allowRestore: boolean;
  allowComment: boolean;
  constructor(loginUser?: User) {
    super();
    if (!loginUser) {
      return;
    }

    this.allowViewOverall = loginUser.hasPermission(
      ActionKey.LearningPlanOverallView
    );
    this.allowCreate = loginUser.hasPermission(ActionKey.LearningPlanCUD);
    this.allowEdit = loginUser.hasPermission(ActionKey.LearningPlanCUD);
    this.allowDelete = loginUser.hasPermission(ActionKey.LearningPlanCUD);
    this.allowSubmit = loginUser.hasPermission(ActionKey.LearningPlanCUD);
    this.allowApprove = loginUser.hasPermission(ActionKey.LearningPlanReview);
    this.allowReject = loginUser.hasPermission(ActionKey.LearningPlanReview);
    this.allowRestore = loginUser.hasPermission(ActionKey.LearningPlanCUD);
    this.allowComment = loginUser.hasPermission(ActionKey.LearningPlanCUD);
  }
}

export enum ActionKey {
  LearningPlanOverallView = 'CompetenceSpa.OrganisationalDevelopment.OrganisationalPDJourney.ViewOverallOfLearningPlan',
  LearningPlanCUD = 'CompetenceSpa.OrganisationalDevelopment.OrganisationalPDJourney.LearningPlan.CUD',
  LearningPlanReview = 'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewLearningPlan',
}
