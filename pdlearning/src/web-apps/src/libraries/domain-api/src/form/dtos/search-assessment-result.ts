import { Assessment, IAssessment } from '../models/assessment.model';

export interface ISearchAssessmentResult {
  items: IAssessment[];
  totalCount: number;
}

export class SearchAssessmentResult implements ISearchAssessmentResult {
  public items: Assessment[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchAssessmentResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new Assessment(item));
    this.totalCount = data.totalCount;
  }
}
