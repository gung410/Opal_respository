import {
  IParticipantAssignmentTrackQuizQuestionAnswer,
  ParticipantAssignmentTrackQuizQuestionAnswer
} from './participant-assignment-track-quiz-question-answer.model';

export interface IParticipantAssignmentTrackQuizAnswer {
  id: string;
  quizAssignmentFormId: string;
  score: number;
  scorePercentage: number;
  questionAnswers: IParticipantAssignmentTrackQuizQuestionAnswer[];
}

export class ParticipantAssignmentTrackQuizAnswer implements IParticipantAssignmentTrackQuizAnswer {
  public id: string;
  public quizAssignmentFormId: string;
  public score: number;
  public scorePercentage: number;
  public questionAnswers: ParticipantAssignmentTrackQuizQuestionAnswer[];

  constructor(data?: IParticipantAssignmentTrackQuizAnswer) {
    if (data) {
      this.id = data.id;
      this.quizAssignmentFormId = data.quizAssignmentFormId;
      this.score = data.score;
      this.scorePercentage = data.scorePercentage;
      this.questionAnswers =
        data.questionAnswers != null ? data.questionAnswers.map(p => new ParticipantAssignmentTrackQuizQuestionAnswer(p)) : [];
    }
  }
}
