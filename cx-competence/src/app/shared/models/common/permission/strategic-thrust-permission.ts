import { User } from 'app-models/auth.model';
import { BaseUserPermission } from './permission-setting';

export interface IStrategicThrustPermission {
  strategicThrustPermission: any;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initStrategicThrustPermission(loginUser: User): void;
}

export class StrategicThrustPermission extends BaseUserPermission {
  constructor(loginUser?: User) {
    super();
    if (!loginUser) {
      return;
    }

    this.allowCreate = loginUser.hasPermission(ActionKey.StrategicThrustsCUD);
    this.allowEdit = loginUser.hasPermission(ActionKey.StrategicThrustsCUD);
    this.allowDelete = loginUser.hasPermission(ActionKey.StrategicThrustsCUD);
    this.allowSubmit = loginUser.hasPermission(ActionKey.StrategicThrustsCUD);
    this.allowApprove = loginUser.hasPermission(ActionKey.StrategicThrustsCUD);
    this.allowReject = loginUser.hasPermission(ActionKey.StrategicThrustsCUD);
  }
}

export enum ActionKey {
  StrategicThrustsCUD = 'CompetenceSpa.OrganisationalDevelopment.StrategicThrusts.CUD',
}
