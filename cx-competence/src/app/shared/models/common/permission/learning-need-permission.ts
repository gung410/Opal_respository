import { User } from 'app-models/auth.model';
import { IDPMode } from 'app/individual-development/idp.constant';
import { BaseUserPermission, IUserPermission } from './permission-setting';

export interface ILearningNeedPermission {
  learningNeedPermission: LearningNeedPermission;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initLearningNeedPermission(loginUser: User): void;
}

export class LearningNeedPermission extends BaseUserPermission {
  chart: IUserPermission;
  comment: IUserPermission;
  careerAspiration: IUserPermission;
  constructor(loginUser?: User, idpMode?: IDPMode) {
    super();
    if (!loginUser) {
      return;
    }
    if (idpMode === IDPMode.Learner) {
      this.allowView = true;
      //TODO: need to check permission

      this.chart = {
        allowView: loginUser.hasPermission(ActionKey.ChartView),
      } as IUserPermission;
      this.comment = {
        allowView: loginUser.hasPermission(ActionKey.CommentView),
        allowCreate: loginUser.hasPermission(ActionKey.CommentAdd),
        allowEdit: loginUser.hasPermission(ActionKey.CommentEdit),
        allowDelete: loginUser.hasPermission(ActionKey.CommentDelete),
      } as IUserPermission;
      this.careerAspiration = {
        allowView: loginUser.hasPermission(ActionKey.CareerAspirationView),
      } as IUserPermission;
    } else {
      this.allowView = loginUser.hasPermission(ActionKey.LearningNeed);
      this.allowApprove = loginUser.hasPermission(ActionKey.LearningNeedReview);
      this.allowReject = loginUser.hasPermission(ActionKey.LearningNeedReview);

      this.chart = {
        allowView: true,
      } as IUserPermission;
      this.comment = {
        allowView: true,
        allowCreate: true,
        allowEdit: true,
        allowDelete: true,
      } as IUserPermission;
      this.careerAspiration = {
        allowView: true,
      } as IUserPermission;
    }
  }
}

export enum ActionKey {
  LearningNeed = 'CompetenceSpa.StaffDetails.LearningNeedsTab',
  LearningneedView = 'Learner.PDPlan',
  LearningNeedReview = 'CompetenceSpa.StaffDetails.LearningNeedsTab.ReviewLNA',
  LearningNeedAnalysisReview = 'Learner.PDPlan.LearningNeedsAnalysis',
  LearningNeedChartView = 'Learner.PDPlan.LearningNeedsAnalysis',
  CareerAspirationView = 'Learner.PDPlan.LearningNeeds.Chart',
  ChartView = 'Learner.PDPlan.LearningNeeds',
  CommentAdd = 'Learner.PDPlan.Comment',
  CommentView = 'Learner.PDPlan.Comment',
  CommentEdit = 'Learner.PDPlan.Comment',
  CommentDelete = 'Learner.PDPlan.Comment',
}
