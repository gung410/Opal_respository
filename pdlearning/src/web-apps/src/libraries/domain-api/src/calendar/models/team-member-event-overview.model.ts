export interface ITeamMemberEventOverviewModel {
  id: string;
  title: string;
  startAt: Date;
  endAt: Date;
}

export class TeamMemberEventOverviewModel implements ITeamMemberEventOverviewModel {
  public id: string;
  public title: string;
  public startAt: Date;
  public endAt: Date;

  constructor(data: ITeamMemberEventOverviewModel) {
    this.id = data.id;
    this.title = data.title;
    this.startAt = data.startAt;
    this.endAt = data.endAt;
  }
}
