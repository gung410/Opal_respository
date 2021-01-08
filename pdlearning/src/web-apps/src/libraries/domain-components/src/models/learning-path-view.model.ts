import { ILearningPathModel, LearningPathModel } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface ILearningPathViewModel extends ILearningPathModel {
  selected: boolean;
  bookmarkNumber: number;
}

export class LearningPathViewModel extends LearningPathModel implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public bookmarkNumber: number;

  public static createFromModel(
    learningPathModel: LearningPathModel,
    bookmarkNumber: number,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {}
  ): LearningPathViewModel {
    return new LearningPathViewModel({
      ...learningPathModel,
      selected: checkAll || selecteds[learningPathModel.id],
      bookmarkNumber: bookmarkNumber
    });
  }

  constructor(data?: ILearningPathViewModel) {
    super(data);
    if (data != null) {
      this.bookmarkNumber = data.bookmarkNumber;
      this.selected = data.selected;
    }
  }
}
