export interface IComment {
  id: string;
  content: string;
  userId: string;
  objectId: string;
  createdDate: string;
  changedDate: string;
  action?: string;
}

export class Comment implements IComment {
  public id: string;
  public content: string;
  public userId: string;
  public objectId: string;
  public createdDate: string;
  public changedDate: string;
  public action?: string;
  constructor(data: IComment) {
    this.id = data.id;
    this.id = data.id;
    this.content = data.content;
    this.userId = data.userId;
    this.objectId = data.objectId;
    this.createdDate = data.createdDate;
    this.changedDate = data.changedDate;
    this.action = data.action;
  }
}
