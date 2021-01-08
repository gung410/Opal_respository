import { User } from 'app-models/auth.model';
import { BaseUserPermission } from './permission-setting';

export interface ILearningDirectionPermission {
  learningDirectionPermission: LearningDirectionPermission;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initLearningDirectionPermission(loginUser: User): void;
}

export class LearningDirectionPermission extends BaseUserPermission {
  allowDuplicate: boolean;
  allowComment: boolean;
  constructor(loginUser?: User) {
    super();
    if (!loginUser) {
      return;
    }

    this.allowCreate = loginUser.hasPermission(ActionKey.LearningDirectionCUD);
    this.allowEdit = loginUser.hasPermission(ActionKey.LearningDirectionCUD);
    this.allowDelete = loginUser.hasPermission(ActionKey.LearningDirectionCUD);
    this.allowSubmit = loginUser.hasPermission(ActionKey.LearningDirectionCUD);
    this.allowApprove = loginUser.hasPermission(
      ActionKey.LearningDirectionReview
    );
    this.allowReject = loginUser.hasPermission(
      ActionKey.LearningDirectionReview
    );
    this.allowDuplicate = loginUser.hasPermission(
      ActionKey.LearningDirectionCUD
    );
    this.allowComment = loginUser.hasPermission(ActionKey.LearningDirectionCUD);
  }
}

export enum ActionKey {
  LearningDirectionCUD = 'CompetenceSpa.OrganisationalDevelopment.OrganisationalPDJourney.LearningDirection.CUD',
  LearningDirectionReview = 'CompetenceSpa.PendingRequests.OrganisationalDevelopment.ReviewLearningDirection',
}
