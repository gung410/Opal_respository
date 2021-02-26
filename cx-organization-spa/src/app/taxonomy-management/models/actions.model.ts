import { ActionsModel, ActionToolbarModel } from '@conexus/cx-angular-common';
import { TaxonomyActionButtonEnum } from '../constant/taxonomy-action-button.enum';

export class TaxonomyActionsModel {
  text: string;
  actionType?: TaxonomyActionButtonEnum;
  icon?: string;
  message?: string;
  allowActionSingle: boolean;
  disable?: boolean;
  constructor(data?: Partial<TaxonomyActionsModel>) {
    if (!data) {
      return;
    }
    this.text = data.text ? data.text : '';
    this.actionType = data.actionType ? data.actionType : undefined;
    this.icon = data.icon ? data.icon : '';
    this.message = data.message ? data.message : '';
    this.allowActionSingle = data.allowActionSingle
      ? data.allowActionSingle
      : undefined;
    this.disable = data.disable ? data.disable : false;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class TaxonomyActionToolbarModel {
  listEssentialActions: ActionsModel[];
  listNonEssentialActions: ActionsModel[];
  listSpecifyActions: ActionsModel[];
  constructor(data?: Partial<ActionToolbarModel>) {
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
