import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';

export class ActionsModel {
  text: string;
  actionType?: StatusActionTypeEnum;
  icon?: string;
  message?: string;
  allowActionSingle: boolean;
  disable?: boolean;
  constructor(data?: Partial<ActionsModel>) {
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
