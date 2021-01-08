import { User } from 'app-models/auth.model';
import { IDPMode } from 'app/individual-development/idp.constant';
import { BaseUserPermission } from './permission-setting';
export interface IEPortfolioPermission {
  ePortfolioPermission: EPortfolioPermission;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initEPortfolioPermission(loginUser: User): void;
}
export class EPortfolioPermission extends BaseUserPermission {
  constructor(loginUser?: User, idpMode?: IDPMode) {
    super();
    if (!loginUser) {
      return;
    }
    if (idpMode === IDPMode.Learner) {
      this.allowView = true;
      //TODO: need to check permission
    } else {
      this.allowView = loginUser.hasPermission(ActionKey.EPortfolio);
    }
  }
}

export enum ActionKey {
  EPortfolio = 'CompetenceSpa.StaffDetails.E-PortfolioTab',
}
