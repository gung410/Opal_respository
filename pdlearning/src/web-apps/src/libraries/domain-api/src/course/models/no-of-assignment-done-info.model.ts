export interface INoOfAssignmentDoneInfo {
  registrationId: string;
  totalAssignments: number;
  doneAssignments: number;
}

export class NoOfAssignmentDoneInfo implements INoOfAssignmentDoneInfo {
  public registrationId: string;
  public totalAssignments: number;
  public doneAssignments: number;

  constructor(data?: INoOfAssignmentDoneInfo) {
    if (data) {
      this.registrationId = data.registrationId;
      this.totalAssignments = data.totalAssignments;
      this.doneAssignments = data.doneAssignments;
    }
  }

  public displayAsText(): string {
    return `${this.doneAssignments}/${this.totalAssignments}`;
  }
}
