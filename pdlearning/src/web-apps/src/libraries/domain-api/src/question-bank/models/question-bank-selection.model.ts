import { QuestionBank } from './question-bank';

export interface IQuestionBankSelection {
  listQuestion: QuestionBank[];
  priority: number;
  minorPriority: number;
}
