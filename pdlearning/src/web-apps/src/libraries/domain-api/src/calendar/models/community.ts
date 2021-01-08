export interface ICommunityModel {
  id: string;
  title: string;
  parentId: string;
}
export class CommunityModel implements ICommunityModel {
  public id: string;
  public title: string;
  public parentId: string;
  constructor(data: ICommunityModel) {
    this.id = data.id;
    this.parentId = data.parentId;
    this.title = data.title;
  }
}
