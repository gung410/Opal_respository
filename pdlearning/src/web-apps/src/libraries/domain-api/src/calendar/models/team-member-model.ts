export interface ITeamMemberModel {
  learnerId: string;
  learnerName: string;
}

export class TeamMemberModel {
  public learnerId: string;
  public learnerName: string;

  constructor(data: ITeamMemberModel) {
    this.learnerId = data.learnerId;
    this.learnerName = data.learnerName;
  }
}
