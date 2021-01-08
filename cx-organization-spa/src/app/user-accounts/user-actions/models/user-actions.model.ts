import { ActionsModel } from './actions.model';

export class UserActionsModel {
  listEssentialActions: ActionsModel[];
  listNonEssentialActions: ActionsModel[];
  listSpecifyActions: ActionsModel[];
  constructor(data?: Partial<UserActionsModel>) {
    if (!data) {
      return;
    }
    this.listEssentialActions = data.listEssentialActions
      ? data.listEssentialActions
      : [];
    this.listNonEssentialActions = data.listNonEssentialActions
      ? data.listNonEssentialActions
      : [];
    this.listSpecifyActions = data.listSpecifyActions
      ? data.listSpecifyActions
      : [];
  }
}
