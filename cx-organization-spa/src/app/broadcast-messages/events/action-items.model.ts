import { ActionsModel } from '@conexus/cx-angular-common';

export class ActionsItemModel<T> {
  action: ActionsModel;
  item: T;
  constructor(data?: Partial<ActionsItemModel<T>>) {
    if (data == null) {
      return;
    }
    this.action = data.action;
    this.item = data.item;
  }
}
