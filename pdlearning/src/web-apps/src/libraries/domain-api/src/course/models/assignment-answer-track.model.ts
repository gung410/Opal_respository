import { AssignmentQuestionAnswerValue } from './quiz-assignment-form-question.model';

export class AssignmentAnswerTrack {
  public questionId: string;
  public questionAnswer: AssignmentQuestionAnswerValue;
  public correctAnswer: AssignmentQuestionAnswerValue;
  public questionOptionCorrectDic: Dictionary<boolean>;
  public manualScore?: number;
  public isCorrect: boolean;
  public score?: number;
  public giveScore?: number;
  public submitedDate?: Date;
}
