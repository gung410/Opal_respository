export interface INoOfAssessmentDoneInfo {
  participantAssignmentTrackId: string;
  totalAssessments: number;
  doneAssessments: number;
}

export class NoOfAssessmentDoneInfo implements INoOfAssessmentDoneInfo {
  public participantAssignmentTrackId: string;
  public totalAssessments: number;
  public doneAssessments: number;

  constructor(data?: INoOfAssessmentDoneInfo) {
    if (data) {
      this.participantAssignmentTrackId = data.participantAssignmentTrackId;
      this.totalAssessments = data.totalAssessments;
      this.doneAssessments = data.doneAssessments;
    }
  }

  public displayAsText(): string {
    return `${this.doneAssessments}/${this.totalAssessments}`;
  }
}
