export interface ITeamMemberEventModel {
  id: string;
  title: string;
  subTitle: string;
  startAt: Date;
  endAt: Date;
  parentId: string;
}

export class TeamMemberEventModel implements ITeamMemberEventModel {
  public id: string;
  public title: string;
  public subTitle: string;
  public startAt: Date;
  public endAt: Date;
  public parentId: string;

  constructor(data: ITeamMemberEventModel) {
    this.id = data.id;
    this.title = data.title;
    this.subTitle = data.subTitle;
    this.startAt = data.startAt;
    this.endAt = data.endAt;
    this.parentId = data.parentId;
  }
}
