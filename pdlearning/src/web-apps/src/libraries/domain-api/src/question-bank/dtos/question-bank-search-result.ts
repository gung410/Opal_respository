import { IQuestionBank, QuestionBank } from '../models/question-bank';

export interface IQuestionBankSearchResult {
  totalCount: number;
  items: IQuestionBank[];
}

export class QuestionBankSearchResult implements IQuestionBankSearchResult {
  public totalCount: number;
  public items: QuestionBank[];

  constructor(data: IQuestionBankSearchResult) {
    if (!data) {
      return;
    }

    this.totalCount = data.totalCount;
    this.items = data.items.map(p => new QuestionBank(p));
  }
}
