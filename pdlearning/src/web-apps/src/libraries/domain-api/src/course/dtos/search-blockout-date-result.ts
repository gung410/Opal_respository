import { BlockoutDateModel } from './../models/blockout-date.model';

export interface ISearchBlockoutDateResult {
  items: BlockoutDateModel[];
  totalCount: number;
}

export class SearchBlockoutDateResult implements ISearchBlockoutDateResult {
  public items: BlockoutDateModel[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchBlockoutDateResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new BlockoutDateModel(item));
    this.totalCount = data.totalCount;
  }
}
