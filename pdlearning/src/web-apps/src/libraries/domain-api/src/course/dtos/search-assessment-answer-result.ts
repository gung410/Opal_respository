import { AssessmentAnswer, IAssessmentAnswer } from '../models/assessment-answer.model';

export interface ISearchAssessmentAnswerResult {
  items: IAssessmentAnswer[];
  totalCount: number;
}

export class SearchAssessmentAnswerResult implements ISearchAssessmentAnswerResult {
  public items: AssessmentAnswer[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchAssessmentAnswerResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new AssessmentAnswer(item));
    this.totalCount = data.totalCount;
  }
}
