import { AssignmentQuestionAnswerValue } from './quiz-assignment-form-question.model';

export interface IParticipantAssignmentTrackQuizQuestionAnswer {
  id: string;
  quizAssignmentFormQuestionId: string;
  quizAnswerId: string;
  answerValue: AssignmentQuestionAnswerValue;
  manualScore?: number;
  manualScoredBy?: string;
  score?: number;
  scoreBy?: string;
  submittedDate?: Date;
}

export class ParticipantAssignmentTrackQuizQuestionAnswer implements IParticipantAssignmentTrackQuizQuestionAnswer {
  public id: string;
  public quizAssignmentFormQuestionId: string;
  public quizAnswerId: string;
  public answerValue: AssignmentQuestionAnswerValue;
  public manualScore?: number;
  public manualScoredBy?: string;
  public score?: number;
  public scoreBy?: string;
  public submittedDate?: Date;
  constructor(data?: IParticipantAssignmentTrackQuizQuestionAnswer) {
    if (data) {
      this.id = data.id;
      this.quizAssignmentFormQuestionId = data.quizAssignmentFormQuestionId;
      this.quizAnswerId = data.quizAnswerId;
      this.answerValue = data.answerValue;
      this.manualScore = data.manualScore;
      this.manualScoredBy = data.manualScoredBy;
      this.score = data.score;
      this.scoreBy = data.scoreBy;
      this.submittedDate = data.submittedDate ? new Date(data.submittedDate) : undefined;
    }
  }

  public get givedScore(): number {
    return this.manualScore != null ? this.manualScore : this.score;
  }
}
