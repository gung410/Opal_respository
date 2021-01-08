import { ActionsModel } from './actions.model';

export class ActionToolbarModel {
  listEssentialActions: ActionsModel[];
  listNonEssentialActions: ActionsModel[];
  listSpecifyActions: ActionsModel[];
  constructor(data?: Partial<ActionToolbarModel>) {
    if (!data) {
      return;
    }
    this.listEssentialActions = data.listEssentialActions ? data.listEssentialActions : [];
    this.listNonEssentialActions = data.listNonEssentialActions ? data.listNonEssentialActions : [];
    this.listSpecifyActions = data.listSpecifyActions ? data.listSpecifyActions : [];
  }
}


