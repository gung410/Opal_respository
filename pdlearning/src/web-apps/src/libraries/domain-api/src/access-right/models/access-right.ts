export interface IAccessRight {
  id: string;
  objectId: string;
  userId: string;
  createdDate: string;
}

export class AccessRight implements IAccessRight {
  public id: string;
  public objectId: string;
  public userId: string;
  public createdDate: string;

  constructor(data?: IAccessRight) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.objectId = data.objectId;
    this.userId = data.userId;
    this.createdDate = data.createdDate;
  }
}
