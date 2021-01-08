export interface IMyLearningPagingModel<T> {
  total: number;
  items: T[];
}
export class MyLearningPagingModel<T> implements IMyLearningPagingModel<T> {
  public total: number;
  public items: T[];
  constructor(data?: IMyLearningPagingModel<T>) {
    if (data == null) {
      return;
    }
    this.total = data.total;
    this.items = data.items;
  }
}
