import { ISearchTag, SearchTag } from '../models/search-tag.model';

export interface IQuerySearchTagResult {
  totalCount: number;
  items: ISearchTag[];
}

export class QuerySearchTagResult implements IQuerySearchTagResult {
  public totalCount: number;
  public items: SearchTag[];

  constructor(data: IQuerySearchTagResult) {
    if (!data) {
      return;
    }

    this.totalCount = data.totalCount;
    this.items = data.items.map(p => new SearchTag(p));
  }
}
