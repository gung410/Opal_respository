import { AssessmentAnswer } from '../models/assessment-answer.model';

export interface ICreateAssessmentAnswerRequest {
  data: CreateAssessmentAnswerRequestData;
}

export class CreateAssessmentAnswerRequestData {
  public id?: string;
  public assessmentId: string;
  public participantAssignmentTrackId: string;
  public userId: string;

  constructor(data?: AssessmentAnswer) {
    if (data != null) {
      this.id = data.id;
      this.assessmentId = data.assessmentId;
      this.participantAssignmentTrackId = data.participantAssignmentTrackId;
      this.userId = data.userId;
    }
  }
}
