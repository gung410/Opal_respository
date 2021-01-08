import { ILearningPathModel, LearningPathModel } from './../models/learning-path.model';

import { Utils } from '@opal20/infrastructure';

export interface ISearchLearningPathResult {
  items: ILearningPathModel[];
  totalCount: number;
}

export class SearchLearningPathResult implements ISearchLearningPathResult {
  public items: LearningPathModel[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchLearningPathResult) {
    if (data == null) {
      return;
    }

    this.items = Utils.orderBy(data.items.map(item => new LearningPathModel(item)), p => p.createdDate, true);
    this.totalCount = data.totalCount;
  }
}
