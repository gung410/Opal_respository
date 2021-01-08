export interface ISelectUserDialogResult {
  id: string;
}

export class SelectUserDialogResult implements ISelectUserDialogResult {
  public id: string;
  constructor(data?: ISelectUserDialogResult) {
    if (data == null) {
      return;
    }
  }
}
