export interface IDigitalContentExpiryInfoModel {
  id: string;
  expiredDate: Date;
}

export class DigitalContentExpiryInfoModel implements IDigitalContentExpiryInfoModel {
  public id: string;
  public expiredDate: Date;

  constructor(data: IDigitalContentExpiryInfoModel) {
    if (data != null) {
      this.id = data.id;
      this.expiredDate = new Date(data.expiredDate);
    }
  }
}
