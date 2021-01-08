import { User } from 'app-models/auth.model';
import { BaseUserPermission } from './permission-setting';

export interface IAdhocNominationPermission {
  adhocNominationPermission: AdhocNominationPermission;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initAdhocNominationPermission(loginUser: User): void;
}

export class AdhocNominationPermission extends BaseUserPermission {
  allowIndividualNominate: boolean;
  allowGroupNominate: boolean;
  allowCurrentOrganisationUnitNominate: boolean;
  allowMassNominate: boolean;
  constructor(loginUser?: User) {
    super();
    if (!loginUser) {
      return;
    }

    this.allowIndividualNominate = loginUser.hasPermission(
      ActionKey.AdhocNomination
    );
    this.allowGroupNominate = loginUser.hasPermission(
      ActionKey.AdhocNomination
    );
    this.allowCurrentOrganisationUnitNominate = loginUser.hasPermission(
      ActionKey.AdhocNomination
    );
    this.allowMassNominate = loginUser.hasPermission(ActionKey.AdhocNomination);
  }
}

export enum ActionKey {
  AdhocNomination = 'CompetenceSpa.AdhocNominations.Implement',
}
