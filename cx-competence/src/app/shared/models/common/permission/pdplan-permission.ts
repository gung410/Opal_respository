import { User } from 'app-models/auth.model';
import { IDPMode } from './../../../../individual-development/idp.constant';
import { BaseUserPermission, IUserPermission } from './permission-setting';
export interface IPDPlanPermission {
  pdPlanPermission: PDPlanPermission;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initPDPlanPermission(loginUser: User): void;
}
export class PDPlanPermission extends BaseUserPermission {
  pdOpportunity: IUserPermission;
  externalPDOpportunity: IUserPermission;
  comment: IUserPermission;
  constructor(loginUser?: User, idpMode?: IDPMode) {
    super();
    if (!loginUser) {
      return;
    }
    if (idpMode === IDPMode.Learner) {
      this.allowView = loginUser.hasPermission(LearnerActionKey.PDPlanView);
      this.allowSubmit = loginUser.hasPermission(LearnerActionKey.PDPlanSubmit);
      this.pdOpportunity = {
        allowCreate: loginUser.hasPermission(LearnerActionKey.PDOpportunityAdd),
        allowDelete: loginUser.hasPermission(
          LearnerActionKey.PDOpportunityRemove
        ),
      } as IUserPermission;
      this.comment = {
        allowView: loginUser.hasPermission(LearnerActionKey.CommentView),
        allowCreate: loginUser.hasPermission(LearnerActionKey.CommentAdd),
        allowEdit: loginUser.hasPermission(LearnerActionKey.CommentEdit),
        allowDelete: loginUser.hasPermission(LearnerActionKey.CommentDelete),
      } as IUserPermission;
      this.externalPDOpportunity = {
        allowCreate: loginUser.hasPermission(
          LearnerActionKey.ExternalPDOpportunityAdd
        ),
        allowEdit: loginUser.hasPermission(
          LearnerActionKey.ExternalPDOpportunityEdit
        ),
        allowDelete: loginUser.hasPermission(
          LearnerActionKey.ExternalPDOpportunityRemove
        ),
        allowChangeStatus: loginUser.hasPermission(
          LearnerActionKey.ExternalPDOpportunityChangeStatus
        ),
      } as IUserPermission;
    } else {
      this.allowView = loginUser.hasPermission(ActionKey.PDPlanView);
      this.allowApprove = loginUser.hasPermission(ActionKey.PDPlanReview);
      this.allowReject = loginUser.hasPermission(ActionKey.PDPlanReview);
      this.pdOpportunity = {
        allowCreate: true,
        allowDelete: true,
      } as IUserPermission;
      this.comment = {
        allowView: true,
        allowCreate: true,
        allowEdit: true,
        allowDelete: true,
      } as IUserPermission;
      this.externalPDOpportunity = {
        allowCreate: true,
        allowEdit: true,
        allowDelete: true,
        allowChangeStatus: true,
      } as IUserPermission;
    }
  }
}

export enum ActionKey {
  PDPlanView = 'CompetenceSpa.StaffDetails.PDPlanTab',
  PDPlanReview = 'CompetenceSpa.StaffDetails.PDPlanTab.ReviewPDPlan',
}
export enum LearnerActionKey {
  PDPlanSubmit = 'Learner.PDPlan.YourPDPlan.Submit',
  PDPlanView = 'Learner.PDPlan.YourPDPlan',
  PDOpportunityAdd = 'Learner.PDPlan.YourPDPlan.PDO.CUD',
  PDOpportunityRemove = 'Learner.PDPlan.YourPDPlan.PDO.CUD',
  ExternalPDOpportunityAdd = 'Learner.PDPlan.YourPDPlan.ExternalPDO.CUD',
  ExternalPDOpportunityEdit = 'Learner.PDPlan.YourPDPlan.ExternalPDO.CUD',
  ExternalPDOpportunityRemove = 'Learner.PDPlan.YourPDPlan.ExternalPDO.CUD',
  ExternalPDOpportunityChangeStatus = 'Learner.PDPlan.YourPDPlan.ExternalPDO.MarkCompletion',
  CommentAdd = 'Learner.PDPlan.Comment',
  CommentView = 'Learner.PDPlan.Comment',
  CommentEdit = 'Learner.PDPlan.Comment',
  CommentDelete = 'Learner.PDPlan.Comment',
}
