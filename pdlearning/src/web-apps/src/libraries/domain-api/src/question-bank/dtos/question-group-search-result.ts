import { QuestionGroup } from '../models/question-group';

export interface IQuestionGroupSearchResult {
  totalCount: number;
  items: QuestionGroup[];
}

export class QuestionGroupSearchResult implements IQuestionGroupSearchResult {
  public totalCount: number;
  public items: QuestionGroup[];

  constructor(data: IQuestionGroupSearchResult) {
    if (!data) {
      return;
    }

    this.totalCount = data.totalCount;
    this.items = data.items.map(p => new QuestionGroup(p));
  }
}
