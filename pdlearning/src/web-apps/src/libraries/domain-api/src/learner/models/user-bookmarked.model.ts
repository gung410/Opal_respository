export interface IUserBookMarkedModel {
  itemId: string;
  countTotal: number;
}

export class UserBookMarkedModel implements IUserBookMarkedModel {
  public itemId: string;
  public countTotal: number = 0;

  constructor(data?: IUserBookMarkedModel) {
    if (data == null) {
      return;
    }
    this.itemId = data.itemId;
    this.countTotal = data.countTotal;
  }
}
