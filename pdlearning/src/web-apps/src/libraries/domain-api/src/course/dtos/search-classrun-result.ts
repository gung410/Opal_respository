import { ClassRun, IClassRun } from '../models/classrun.model';

export interface ISearchClassRunResult {
  items: IClassRun[];
  totalCount: number;
}

export class SearchClassRunResult implements ISearchClassRunResult {
  public items: ClassRun[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchClassRunResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new ClassRun(item));
    this.totalCount = data.totalCount;
  }
}
