import { User } from 'app-models/auth.model';
import { IDPMode } from './../../../../individual-development/idp.constant';
import { BaseUserPermission, IUserPermission } from './permission-setting';

export interface ILearningNeedAnalysisPermission {
  learningNeedAnalysisPermission: LearningNeedAnalysisPermission;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initLearningNeedAnalysisPermission(loginUser: User): void;
}

export class LearningNeedAnalysisPermission extends BaseUserPermission {
  chart: IUserPermission;
  careerAspiration: IUserPermission;
  constructor(loginUser?: User, idpMode?: IDPMode) {
    super();
    if (!loginUser) {
      return;
    }
    if (idpMode == IDPMode.Learner) {
      this.allowView = loginUser.hasPermission(
        ActionKey.LearningNeedAnalysisReview
      );
      this.allowReview = loginUser.hasPermission(
        ActionKey.LearningNeedAnalysisReview
      );
      this.chart = {
        allowView: loginUser.hasPermission(ActionKey.ChartView),
      } as IUserPermission;
      this.careerAspiration = {
        allowView: loginUser.hasPermission(ActionKey.ChartView),
      } as IUserPermission;
    } else {
      this.allowReview = true;
      this.chart = {
        allowView: true,
      } as IUserPermission;
      this.careerAspiration = {
        allowView: true,
      } as IUserPermission;
    }
  }
}

export enum ActionKey {
  ChartView = 'Learner.PDPlan.LearningNeeds',
  CareerAspirationView = 'Learner.PDPlan.LearningNeeds.Chart',
  LearningNeedAnalysisReview = 'Learner.PDPlan.LearningNeedsAnalysis',
}
