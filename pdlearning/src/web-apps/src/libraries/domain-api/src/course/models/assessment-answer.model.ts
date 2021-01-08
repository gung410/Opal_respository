import { AssessmentCriteriaAnswer, IAssessmentCriteriaAnswer } from './assessment-criteria-answer.model';

export interface IAssessmentAnswer {
  id?: string;
  assessmentId: string;
  participantAssignmentTrackId: string;
  userId: string;
  submittedDate?: Date;
  changedBy: string;
  changedDate?: Date;
  criteriaAnswers: IAssessmentCriteriaAnswer[];
}

export class AssessmentAnswer implements IAssessmentAnswer {
  public static assessmentForFacilitator = '00000000-0000-0000-0000-000000000000';
  public id?: string;
  public assessmentId: string;
  public participantAssignmentTrackId: string;
  public userId: string;
  public submittedDate?: Date;
  public changedBy: string;
  public changedDate?: Date;
  public criteriaAnswers: AssessmentCriteriaAnswer[] = [];

  constructor(data?: IAssessmentAnswer) {
    if (data) {
      this.id = data.id;
      this.assessmentId = data.assessmentId;
      this.participantAssignmentTrackId = data.participantAssignmentTrackId;
      this.userId = data.userId;
      this.submittedDate = data.submittedDate;
      this.changedBy = data.changedBy;
      this.criteriaAnswers = data.criteriaAnswers != null ? data.criteriaAnswers.map(x => new AssessmentCriteriaAnswer(x)) : [];
      this.changedDate = data.changedDate;
    }
  }

  public isAssessentForFacilitator(): boolean {
    return this.userId === AssessmentAnswer.assessmentForFacilitator;
  }
}
