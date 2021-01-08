export interface IExternalUserInfoModel {
  id: string;
}

export class ExternalUserInfoModel implements IExternalUserInfoModel {
  public id: string;

  constructor(data?: IExternalUserInfoModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
  }
}
