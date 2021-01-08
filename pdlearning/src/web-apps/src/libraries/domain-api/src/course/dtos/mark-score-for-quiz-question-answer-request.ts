import { IMarkScoreForQuizQuestionAnswer } from './mark-score-for-quiz-question-answer-dto';
export interface IMarkScoreForQuizQuestionAnswerRequest {
  markScoreForQuizQuestionAnswers: IMarkScoreForQuizQuestionAnswer[];
  participantAssignmentTrackId: string;
}
