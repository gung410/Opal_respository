export class ActionsModel {
  text: string;
  actionType?: any;
  icon?: string;
  messageConfirm?: string;
  allowActionSingle: boolean;
  disable?: boolean;
  constructor(data?: Partial<ActionsModel>) {
    if (!data) {
      return;
    }
    this.text = data.text ? data.text : '';
    this.actionType = data.actionType ? data.actionType : undefined;
    this.icon = data.icon ? data.icon : '';
    this.messageConfirm = data.messageConfirm ? data.messageConfirm : '';
    this.allowActionSingle = data.allowActionSingle ? data.allowActionSingle : undefined;
  }
}
